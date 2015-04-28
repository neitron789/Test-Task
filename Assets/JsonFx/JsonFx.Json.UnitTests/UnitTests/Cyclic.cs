using System;
using System.IO;

using Pathfinding.Serialization.JsonFx;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Pathfinding.Serialization.JsonFx.Test.UnitTests
{
	public class Cyclic
	{
		public Cyclic ()
		{
		}

		class A {
			public B b;
			public A a;
			public Dictionary<string,A> d;
			public A[] q = new A[100];
		}

		class B {
			public A a;
			public A a2;
			public B b2;
			public A[] q = new A[100];
		}

		struct Z {
			public A a;
			public B b;
		}


		public static void RunTest (TextWriter writer, string unitTestsFolder, string outputFolder)
		{
			JsonWriterSettings wsettings = new JsonWriterSettings();
			wsettings.HandleCyclicReferences = true;
			wsettings.PrettyPrint = true;

			A a = new A();
			a.b = new B();
			a.b.a = a;
			a.a = a;
			a.b.a2 = new A();
			a.b.a2.a = a;
			a.b.b2 = new B();
			a.b.b2.a = a.b.a2;
			a.b.b2.b2 = new B();
			a.b.b2.b2.b2 = new B();
			a.b.b2.b2.b2.b2 = new B();
			a.b.b2.b2.b2.b2.b2 = new B();
			a.b.b2.b2.b2.b2.b2.b2 = new B();
			a.b.b2.b2.b2.b2.b2.b2.b2 = a.b.b2.b2;
			a.b.b2.b2.b2.b2.b2.b2.a = a;

			a.q[2] = a;
			a.b.a2.q = a.q;

			a.d = new Dictionary<string, A>();
			a.d["blah"] = a;
			a.d["meh"] = a.b.a2;

			A[] arr = new A[5];
			arr[0] = a;
			arr[1] = a;
			arr[2] = new A();
			arr[3] = arr[2];
			arr[2].a = a;
			arr[2].b = a.b;
			for (int i=4;i<5;i++) arr[i] = new A();

			using (StreamWriter wr2 = new StreamWriter("out", false, Encoding.UTF8))
			{
				JsonWriter wr = new JsonWriter(wr2, wsettings);
				wr.Write(a);
				wr.Write(arr);
			}

			using (StreamReader re = new StreamReader ("out", Encoding.UTF8)) {

				JsonReaderSettings rsettings = new JsonReaderSettings ();
				rsettings.HandleCyclicReferences = true;

				JsonReader read = new JsonReader (re, rsettings);
				a = (A)read.Deserialize(typeof(A));

				// Do some checking
				if (a == null || a.a != a || a.b.a != a ||
				    a.b.a2.a != a || a.b.b2.a != a.b.a2 ||
				    a.b.b2.b2.b2.b2.b2.b2.b2 != a.b.b2.b2 ||
				    a.d["meh"] != a.b.a2 || a.d["blah"] != a) {
					throw new System.Exception ("Invalid, could not deserialize or serialize cyclic classes correctly.");
				}


				object ob = read.Deserialize(typeof(A[]));

				arr = (A[])ob;

				if (arr[0] != a || arr[3] != arr[2]) {
					throw new System.Exception ("Invalid, Could not serialize or deserialize array correctly");
				}
			}
			//JsonReaderSettings rsettings = new JsonReaderSettings ();
			//rsettings.
			//JsonReader reader = new JsonReader ();

		}
	}
}

