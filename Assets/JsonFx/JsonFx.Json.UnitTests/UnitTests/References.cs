using System;
using System.Text;
using Pathfinding.Serialization.JsonFx;
using System.Collections.Generic;
using System.IO;

namespace Pathfinding.Serialization.JsonFx.Test.UnitTests
{
	public class References
	{

		[JsonHandledReferenceAttribute]
		class Root {
			[JsonOwnedReference]
			public B ownedReference;
			public Root other;
			public Dictionary<string,Root> dictToOther;
			public List<Root> listToOther;
			public Root[] arrayToOther;
		}

		[JsonHandledReferenceAttribute]
		class B {
			public Root nestedRefToOther;
		}

		public static void RunTest (TextWriter writer, string unitTestsFolder, string outputFolder) {

			var watch = System.Diagnostics.Stopwatch.StartNew ();

			JsonWriterSettings wsettings = new JsonWriterSettings();
			wsettings.PrettyPrint = true;

			Root a = new Root();
			a.ownedReference = new B();

			// Self
			a.other = a;

			a.dictToOther = new Dictionary<string, Root>();
			a.dictToOther["refToSelf"] = a;

			Root late = new Root();
			late.other = a;
			late.dictToOther = new Dictionary<string, Root> ();
			late.dictToOther["refToFirst"] = a;
			late.dictToOther["refToFirst2"] = a;
			late.dictToOther["refToSelf"] = late;

			a.ownedReference.nestedRefToOther = late;
			a.listToOther = new List<Root> ();
			a.listToOther.Add (a);
			a.listToOther.Add (late);
			a.arrayToOther = new Root[] { a, late };

			late.listToOther = new List<Root> ();
			late.listToOther.Add (a);
			late.listToOther.Add (late);
			late.arrayToOther = new Root[] { a, late };

			for (int i = 0; i < 100; i++) {
				late.dictToOther [i.ToString ()] = a;
			}

			var refHandler = new ReferenceHandlerWriter ();

			using (StreamWriter wr2 = new StreamWriter(outputFolder+"/References1", false, Encoding.UTF8))
			{
				JsonWriter wr = new JsonWriter(wr2, wsettings);
				wr.referenceHandler = refHandler;
				wr.Write(a);
			}

			using (StreamWriter wr2 = new StreamWriter(outputFolder+"/References2", false, Encoding.UTF8))
			{
				JsonWriter wr = new JsonWriter(wr2, wsettings);
				wr.referenceHandler = refHandler;

				wr.Write(late);
			}

			if (refHandler.GetNonSerializedReferences ().Count != 0) {
				throw new System.Exception ("Not all references were serialized");
			}

			var refHandlerRead = new ReferenceHandlerReader ();

			using (StreamReader re = new StreamReader (outputFolder+"/References2", Encoding.UTF8)) {
				JsonReaderSettings rsettings = new JsonReaderSettings ();

				JsonReader read = new JsonReader (re, rsettings);
				read.referenceHandler = refHandlerRead;

				late = (Root)read.Deserialize(typeof(Root));

				if (late.arrayToOther[0] != null) {
					throw new System.Exception ("Array item should not have been deserialized yet");
				}

				if (late.listToOther[0] != null) {
					throw new System.Exception ("List item should not have been deserialized yet");
				}
			}

			using (StreamReader re = new StreamReader (outputFolder+"/References1", Encoding.UTF8)) {

				JsonReaderSettings rsettings = new JsonReaderSettings ();

				JsonReader read = new JsonReader (re, rsettings);
				read.referenceHandler = refHandlerRead;

				a = (Root)read.Deserialize(typeof(Root));

				// Do some checking
				if (a.other != a || a.ownedReference.nestedRefToOther != late || a.dictToOther ["refToSelf"] != a) {
					throw new System.Exception ("Failed to serialize or deserialize references correctly");
				}

				if (late.other != a) {
					throw new System.Exception ("Failed to handle out of order references (case 1)");
				}

				if (late.dictToOther ["refToSelf"] != late) {
					throw new System.Exception ("Failed to handle out of order references (case 2)");
				}

				if (late.dictToOther ["refToFirst"] != a || late.dictToOther ["refToFirst2"] != a) {
					throw new System.Exception ("Failed to handle out of order references (case 3)");
				}

				if (late.listToOther[0] != a || late.listToOther[1] != late || a.listToOther[0] != a || late.listToOther[1] != late) {
					throw new System.Exception ("Failed to handle lists");
				}

				if (late.arrayToOther[0] != a || late.arrayToOther[1] != late || a.arrayToOther[0] != a || late.arrayToOther[1] != late) {
					throw new System.Exception ("Failed to handle arrays");
				}
			}

			watch.Stop ();
			//System.Console.WriteLine (watch.Elapsed.TotalMilliseconds.ToString ("0.00"));
		}
	}
}

