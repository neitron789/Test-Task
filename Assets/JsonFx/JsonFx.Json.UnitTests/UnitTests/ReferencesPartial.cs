using System;
using System.Text;
using Pathfinding.Serialization.JsonFx;
using System.Collections.Generic;
using System.IO;

namespace Pathfinding.Serialization.JsonFx.Test.UnitTests
{
	public class ReferencesPartial
	{

		[JsonHandledReferenceAttribute]
		class Root {
			[JsonOwnedReference]
			public B ownedReference;
			public Root other;
			public Dictionary<string,Root> dictToOther;
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

			var refHandler = new ReferenceHandlerWriter ();

			using (StreamWriter wr2 = new StreamWriter(outputFolder+"/ReferencesPartial", false, Encoding.UTF8))
			{
				JsonWriter wr = new JsonWriter(wr2, wsettings);
				wr.referenceHandler = refHandler;

				wr.Write(late);
			}

			if (refHandler.GetNonSerializedReferences ().Count == 0) {
				throw new System.Exception ("All references were serialized. This should not have been the case");
			}

			var refHandlerRead = new ReferenceHandlerReader ();

			refHandler.TransferNonSerializedReferencesToReader (refHandlerRead);

			using (StreamReader re = new StreamReader (outputFolder+"/ReferencesPartial", Encoding.UTF8)) {
				JsonReaderSettings rsettings = new JsonReaderSettings ();

				JsonReader read = new JsonReader (re, rsettings);
				read.referenceHandler = refHandlerRead;

				late = (Root)read.Deserialize(typeof(Root));

				// Do some checking
				if (late.other != a) {
					throw new System.Exception ("Failed to handle out of order references (case 1)");
				}

				if (late.dictToOther ["refToSelf"] != late) {
					throw new System.Exception ("Failed to handle out of order references (case 2)");
				}

				if (late.dictToOther ["refToFirst"] != a || late.dictToOther ["refToFirst2"] != a) {
					throw new System.Exception ("Failed to handle out of order references (case 3)");
				}
			}

			watch.Stop ();
			//System.Console.WriteLine (watch.Elapsed.TotalMilliseconds.ToString ("0.00"));
		}
	}
}

