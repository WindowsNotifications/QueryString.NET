using System;
using System.Collections.Generic;
using System.Linq;

#if TEST_PORTABLE
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#endif

namespace Microsoft.QueryStringDotNET.Test
{
    [TestClass]
    public class QueryStringTest
    {
        [TestMethod]
        public void TestEqualsTrue()
        {
            AssertEqual(new QueryString(), new QueryString());

            AssertEqual(new QueryString()
            {
                { "isBook" }
            }, new QueryString()
            {
                { "isBook" }
            });

            AssertEqual(new QueryString()
            {
                { "isBook" }
            }, new QueryString()
            {
                { "isBook", null }
            });

            AssertEqual(new QueryString()
            {
                { "name", "Andrew" }
            }, new QueryString()
            {
                { "name", "Andrew" }
            });

            AssertEqual(new QueryString()
            {
                { "name", "Andrew" },
                { "age", "22" }
            }, new QueryString()
            {
                { "age", "22" },
                { "name", "Andrew" }
            });

            // Different order of matching name values
            AssertEqual(new QueryString()
            {
                { "name", "Andrew" },
                { "name", null },
                { "name", "Lei" }
            }, new QueryString()
            {
                { "name" },
                { "name", "Lei" },
                { "name", "Andrew" }
            });
        }

        [TestMethod]
        public void TestEqualsFalse()
        {
            AssertNotEqual(new QueryString(), new QueryString()
            {
                { "blah" }
            });

            AssertNotEqual(new QueryString()
            {
                { "isBook" }
            }, new QueryString()
            {
                { "isBook", "true" }
            });

            AssertNotEqual(new QueryString()
            {
                { "name", "Andrew" }
            }, new QueryString()
            {
                { "name", "andrew" }
            });

            AssertNotEqual(new QueryString()
            {
                { "name", "Andrew" }
            }, new QueryString()
            {
                { "Name", "andrew" }
            });

            AssertNotEqual(new QueryString()
            {
                { "name", "Andrew" }
            }, new QueryString()
            {
                { "Name", "Lei" }
            });

            AssertNotEqual(new QueryString()
            {
                { "name", "Andrew" },
                { "name", null },
                { "name", "Lei" }
            }, new QueryString()
            {
                { "name", "Lei" },
                { "name", "Andrew" }
            });

            AssertNotEqual(new QueryString()
            {
                { "name", "Andrew" },
                { "name", null },
                { "name", "Lei" }
            }, new QueryString()
            {
                { "name", "Thomas" },
                { "name", "Lei" },
                { "name", "Andrew" }
            });

            AssertNotEqual(new QueryString()
            {
                { "name", "Andrew" },
                { "name", null },
                { "name", "Lei" }
            }, new QueryString()
            {
                { "name", "Lei" },
                { "name", null },
                { "name", "andrew" }
            });

            AssertNotEqual(new QueryString()
            {
                { "name", "Andrew" },
                { "age", "22" }
            }, new QueryString()
            {
                { "age", "22" },
                { "firstName", "Andrew" }
            });
        }

        [TestMethod]
        public void TestAddExceptions_NullName()
        {
            QueryString query = new QueryString();

            try
            {
                query.Add(null, "value");
            }

            catch (ArgumentNullException)
            {
                return;
            }

            Assert.Fail("Adding null name shouldn't be allowed.");
        }

        [TestMethod]
        public void TestParsing()
        {
            AssertParse(new QueryString(), "");
            AssertParse(new QueryString(), "   ");
            AssertParse(new QueryString(), "\n");
            AssertParse(new QueryString(), "\t \n");
            AssertParse(new QueryString(), null);

            AssertParse(new QueryString()
            {
                { "isBook" }
            }, "isBook");

            AssertParse(new QueryString()
            {
                { "isBook" },
                { "isRead" }
            }, "isBook&isRead");

            AssertParse(new QueryString()
            {
                { "isBook" },
                { "isRead" },
                { "isLiked" }
            }, "isBook&isRead&isLiked");

            AssertParse(new QueryString()
            {
                { "isBook" },
                { "isRead" },
                { "isBook" }
            }, "isBook&isRead&isBook");

            AssertParse(new QueryString()
            {
                { "name", "Andrew" }
            }, "name=Andrew");

            AssertParse(new QueryString()
            {
                { "name", "Andrew" },
                { "isMale" }
            }, "name=Andrew&isMale");

            AssertParse(new QueryString()
            {
                { "name", "Andrew" },
                { "isMale" }
            }, "isMale&name=Andrew");

            AssertParse(new QueryString()
            {
                { "name", "Andrew" },
                { "age", "22" }
            }, "age=22&name=Andrew");

            AssertParse(new QueryString()
            {
                { "name", "Andrew" },
                { "age", "22" },
                { "name", "Lei" }
            }, "age=22&name=Andrew&name=Lei");
        }
        
        [TestMethod]
        public void TestToString_ExactString()
        {
            Assert.AreEqual("", new QueryString().ToString());

            Assert.AreEqual("isBook", new QueryString()
            {
                { "isBook" }
            }.ToString());

            Assert.AreEqual("name=Andrew", new QueryString()
            {
                { "name", "Andrew" }
            }.ToString());
        }

        [TestMethod]
        public void TestToString_SemicolonSeparator()
        {
            Assert.IsTrue(new QueryString()
            {
                { "name", "Andrew" },
                { "age", "22" }
            }.ToString(QueryStringSeparator.Semicolon).Contains(";"));
        }

        [TestMethod]
        public void TestUrlEncoding()
        {
            Assert.AreEqual("full+name=Andrew+Bares", new QueryString()
            {
                { "full name", "Andrew Bares" }
            }.ToString());

            Assert.AreEqual("name%3Bcompany=Andrew%3BMicrosoft", new QueryString()
            {
                { "name;company", "Andrew;Microsoft" }
            }.ToString());

            Assert.AreEqual("name%2Fcompany=Andrew%2FMicrosoft", new QueryString()
            {
                { "name/company", "Andrew/Microsoft" }
            }.ToString());

            Assert.AreEqual("message=Dinner%3F", new QueryString()
            {
                { "message", "Dinner?" }
            }.ToString());

            Assert.AreEqual("message=to%3A+Andrew", new QueryString()
            {
                { "message", "to: Andrew" }
            }.ToString());

            Assert.AreEqual("email=andrew%40live.com", new QueryString()
            {
                { "email", "andrew@live.com" }
            }.ToString());

            Assert.AreEqual("messsage=food%3Dyummy", new QueryString()
            {
                { "messsage", "food=yummy" }
            }.ToString());

            Assert.AreEqual("messsage=%24%24%24", new QueryString()
            {
                { "messsage", "$$$" }
            }.ToString());

            Assert.AreEqual("messsage=-_.%21~%2A%27%28%29", new QueryString()
            {
                { "messsage", "-_.!~*'()" }
            }.ToString());
        }

        [TestMethod]
        public void TestUrlDecoding()
        {
            AssertDecode("Hello world", "Hello+world");
            AssertDecode("Hello world", "Hello%20world");

            AssertDecode(";/?:@&=+$", "%3B%2F%3F%3A%40%26%3D%2B%24");
            AssertDecode("-_.!~*'()", "-_.%21~%2A%27%28%29");
        }

        [TestMethod]
        public void TestIndexer_NullException()
        {
            try
            {
                string val = new QueryString()[null];
            }

            catch (ArgumentNullException)
            {
                return;
            }

            Assert.Fail("Exception should have been thrown.");
        }

        [TestMethod]
        public void TestIndexer_NotFoundException()
        {
            try
            {
                string val = new QueryString()
                {
                    { "name", "Andrew" }
                }["Name"];
            }

            catch (KeyNotFoundException)
            {
                return;
            }

            Assert.Fail("Exception should have been thrown.");
        }

        [TestMethod]
        public void TestIndexer()
        {
            AssertIndexer(null, "isBook&name=Andrew", "isBook");

            AssertIndexer("Andrew", "isBook&name=Andrew", "name");

            AssertIndexer("Andrew", "count=2&name=Andrew&name=Lei", "name");
        }

        [TestMethod]
        public void TestGetValues()
        {
            AssertGetValues(null, "isBook", "name");

            AssertGetValues(new string[] { null }, "isBook", "isBook");

            AssertGetValues(new string[] { "Andrew" }, "isBook&name=Andrew&age=20", "name");

            AssertGetValues(new string[] { "Andrew", "Lei" }, "name=Andrew&name=Lei&age=20", "name");
        }

        [TestMethod]
        public void TestRemove_OnlyKey()
        {
            QueryString qs = new QueryString()
            {
                { "name", "Andrew" },
                { "age", "22" }
            };

            Assert.IsTrue(qs.Remove("age"));

            AssertEqual(new QueryString()
            {
                { "name", "Andrew" }
            }, qs);

            Assert.IsFalse(qs.Remove("age"));
        }

        [TestMethod]
        public void TestRemoveWithValue_OnlyKey()
        {
            QueryString qs = new QueryString()
            {
                { "name", "Andrew" },
                { "age", "22" }
            };

            Assert.IsFalse(qs.Remove("age", null));
            Assert.IsFalse(qs.Remove("age", "21"));
            Assert.IsTrue(qs.Remove("age", "22"));

            AssertEqual(new QueryString()
            {
                { "name", "Andrew" }
            }, qs);

            Assert.IsFalse(qs.Remove("age", "22"));
        }

        [TestMethod]
        public void TestRemove_TwoKeys()
        {
            QueryString qs = new QueryString()
            {
                { "name", "Andrew" },
                { "age", "22" },
                { "age", "23" }
            };

            Assert.IsTrue(qs.Remove("age"));

            AssertEqual(new QueryString()
            {
                { "name", "Andrew" },
                { "age", "23" }
            }, qs);

            Assert.IsTrue(qs.Remove("age"));

            AssertEqual(new QueryString()
            {
                { "name", "Andrew" }
            }, qs);

            Assert.IsFalse(qs.Remove("age"));
        }

        [TestMethod]
        public void TestRemoveWithValue_MultipleKeys()
        {
            QueryString qs = new QueryString()
            {
                { "name", "Andrew" },
                { "name", "Lei" }, // to be removed
                { "name", "Thomas" },
                { "name", "Lei" },
                { "age", "22" }
            };

            Assert.IsFalse(qs.Remove("name", null));
            Assert.IsFalse(qs.Remove("name", "andrew"));
            Assert.IsFalse(qs.Remove("name", "Matt"));
            Assert.IsTrue(qs.Remove("name", "Lei"));

            AssertEqual(new QueryString()
            {
                { "name", "Andrew" }, // to be removed
                { "name", "Thomas" },
                { "name", "Lei" },
                { "age", "22" }
            }, qs);

            Assert.IsTrue(qs.Remove("name", "Andrew"));

            AssertEqual(new QueryString()
            {
                { "name", "Thomas" },
                { "name", "Lei" }, // to be removed
                { "age", "22" }
            }, qs);

            Assert.IsFalse(qs.Remove("name", "Andrew"));
            Assert.IsTrue(qs.Remove("name", "Lei"));

            AssertEqual(new QueryString()
            {
                { "name", "Thomas" },
                { "age", "22" }
            }, qs);

            Assert.IsFalse(qs.Remove("name", "Lei"));
        }

        [TestMethod]
        public void TestRemoveAll_OnlyKey()
        {
            QueryString qs = new QueryString()
            {
                { "name", "Andrew" },
                { "age", "22" }
            };

            Assert.IsTrue(qs.RemoveAll("age"));

            AssertEqual(new QueryString()
            {
                { "name", "Andrew" }
            }, qs);

            Assert.IsFalse(qs.RemoveAll("age"));
        }

        [TestMethod]
        public void TestRemoveAllWithValues()
        {
            QueryString qs = new QueryString()
            {
                { "name", "Andrew" },
                { "age", "22" }
            };

            Assert.AreEqual(0, qs.RemoveAll("age", "21"));
            Assert.AreEqual(0, qs.RemoveAll("age", null));
            Assert.AreEqual(1, qs.RemoveAll("age", "22"));

            AssertEqual(new QueryString()
            {
                { "name", "Andrew" }
            }, qs);

            Assert.AreEqual(0, qs.RemoveAll("age", "22"));

            qs.Add("name", "Lei");
            qs.Add("name", "Andrew");
            qs.Add("name", "Thomas");

            Assert.AreEqual(2, qs.RemoveAll("name", "Andrew"));

            AssertEqual(new QueryString()
            {
                { "name", "Lei" },
                { "name", "Thomas" }
            }, qs);
        }

        [TestMethod]
        public void TestRemoveAll_MultipleKeys()
        {
            QueryString qs = new QueryString()
            {
                { "name", "Andrew" },
                { "name", "Lei" },
                { "name", "Thomas" },
                { "age", "22" }
            };

            Assert.IsTrue(qs.RemoveAll("name"));

            AssertEqual(new QueryString()
            {
                { "age", "22" }
            }, qs);

            Assert.IsFalse(qs.RemoveAll("name"));
        }

        [TestMethod]
        public void TestContains()
        {
            QueryString qs = new QueryString();

            Assert.IsFalse(qs.Contains("name"));
            Assert.IsFalse(qs.Contains("name", "Andrew"));

            qs.Add("isBook");

            Assert.IsFalse(qs.Contains("name"));
            Assert.IsFalse(qs.Contains("name", "Andrew"));

            Assert.IsTrue(qs.Contains("isBook"));
            Assert.IsTrue(qs.Contains("isBook", null));
            Assert.IsFalse(qs.Contains("isBook", "True"));

            qs.Add("isBook", "True");

            Assert.IsTrue(qs.Contains("isBook"));
            Assert.IsTrue(qs.Contains("isBook", null));
            Assert.IsTrue(qs.Contains("isBook", "True"));

            qs.Add("name", "Andrew");

            Assert.IsTrue(qs.Contains("name"));
            Assert.IsFalse(qs.Contains("name", null)); // Value doesn't exist
            Assert.IsTrue(qs.Contains("name", "Andrew"));
            Assert.IsFalse(qs.Contains("Name", "Andrew")); // Wrong case on name
            Assert.IsFalse(qs.Contains("name", "andrew")); // Wrong case on value
            Assert.IsFalse(qs.Contains("Name")); // Wrong case on name
        }

        [TestMethod]
        public void TestSet()
        {
            QueryString qs = new QueryString();

            qs.Set("name", "Andrew");

            AssertEqual(new QueryString()
            {
                { "name", "Andrew" }
            }, qs);

            qs.Set("age", "22");

            AssertEqual(new QueryString()
            {
                { "name", "Andrew" },
                { "age", "22" }
            }, qs);

            qs.Set("name", "Lei");

            AssertEqual(new QueryString()
            {
                { "name", "Lei" },
                { "age", "22" }
            }, qs);

            qs.Add("name", "Thomas");

            AssertEqual(new QueryString()
            {
                { "name", "Lei" },
                { "name", "Thomas" },
                { "age", "22" }
            }, qs);

            qs.Set("name", null);

            AssertEqual(new QueryString()
            {
                { "name" },
                { "age", "22" }
            }, qs);
        }

        [TestMethod]
        public void TestEnumerator()
        {
            QueryStringParameter[] parameters = QueryString.Parse("name=Andrew&age=22&name=Lei&age=27&isOld").ToArray();

            Assert.AreEqual(5, parameters.Length);
            Assert.AreEqual(2, parameters.Count(i => i.Name.Equals("name")));
            Assert.AreEqual(2, parameters.Count(i => i.Name.Equals("age")));
            Assert.AreEqual(1, parameters.Count(i => i.Name.Equals("isOld")));
            Assert.IsTrue(parameters.Any(i => i.Name.Equals("name") && i.Value.Equals("Andrew")));
            Assert.IsTrue(parameters.Any(i => i.Name.Equals("name") && i.Value.Equals("Lei")));
            Assert.IsTrue(parameters.Any(i => i.Name.Equals("age") && i.Value.Equals("22")));
            Assert.IsTrue(parameters.Any(i => i.Name.Equals("age") && i.Value.Equals("27")));
            Assert.IsTrue(parameters.Any(i => i.Name.Equals("isOld") && i.Value == null));
        }

        [TestMethod]
        public void TestCount()
        {
            QueryString qs = new QueryString();
            
            Assert.AreEqual(0, qs.Count());

            qs.Add("name", "Andrew");

            Assert.AreEqual(1, qs.Count());

            qs.Add("age", "22");

            Assert.AreEqual(2, qs.Count());

            qs.Add("name", "Lei");

            Assert.AreEqual(3, qs.Count());

            qs.Set("name", "Thomas");

            Assert.AreEqual(2, qs.Count());

            qs.Add("name", "Matt");

            Assert.AreEqual(3, qs.Count());

            qs.Remove("name", "Matt");

            Assert.AreEqual(2, qs.Count());

            qs.Remove("name", "Thomas");

            Assert.AreEqual(1, qs.Count());

            qs.Remove("age");

            Assert.AreEqual(0, qs.Count());

            qs.Add("name", "Andrew");
            qs.Add("name", "Lei");

            Assert.AreEqual(2, qs.Count());

            qs.RemoveAll("name");

            Assert.AreEqual(0, qs.Count());
        }

        private static void AssertGetValues(string[] expected, string queryString, string paramName)
        {
            string[] actual;

            if (QueryString.Parse(queryString).TryGetValues(paramName, out actual))
            {
                Assert.IsTrue(expected.SequenceEqual(actual), "Expected: " + string.Join(",", expected) + "\nActual: " + string.Join(",", actual));
            }

            else
            {
                Assert.AreEqual(expected, null);
            }
        }

        private static void AssertIndexer(string expected, string queryString, string paramName)
        {
            QueryString q = QueryString.Parse(queryString);

            Assert.AreEqual(expected, q[paramName]);
        }

        private static void AssertDecode(string expected, string encoded)
        {
            Assert.AreEqual(expected, QueryString.Parse("message=" + encoded)["message"]);
        }

        private static void AssertParse(QueryString expected, string inputQueryString)
        {
            Assert.IsTrue(expected.Equals(QueryString.Parse(inputQueryString)), "Expected: " + expected + "\nActual: " + inputQueryString);

            if (inputQueryString != null)
                Assert.IsTrue(expected.Equals(QueryString.Parse(inputQueryString.Replace('&', ';'))), "Expected: " + expected + "\nActual: " + inputQueryString);
        }

        private static void AssertEqual(QueryString expected, QueryString actual)
        {
            Assert.IsTrue(expected.Equals(actual), "Expected: " + expected + "\nActual: " + actual);

            Assert.IsTrue(expected.Equals(QueryString.Parse(actual.ToString())), "After serializing and parsing actual, result changed.\n\nExpected: " + expected + "\nActual: " + QueryString.Parse(actual.ToString()));
            Assert.IsTrue(QueryString.Parse(expected.ToString()).Equals(actual), "After serializing and parsing expected, result changed.\n\nExpected: " + QueryString.Parse(expected.ToString()) + "\nActual: " + actual);
            Assert.IsTrue(QueryString.Parse(expected.ToString()).Equals(QueryString.Parse(actual.ToString())), "After serializing and parsing both, result changed.\n\nExpected: " + QueryString.Parse(expected.ToString()) + "\nActual: " + QueryString.Parse(actual.ToString()));
        }

        private static void AssertNotEqual(QueryString queryString1, QueryString queryString2)
        {
            Assert.IsFalse(queryString1.Equals(queryString2), "First: " + queryString1 + "\nSecond: " + queryString2);

            Assert.IsFalse(queryString1.Equals(QueryString.Parse(queryString2.ToString())), "After serializing and parsing actual, result changed.\n\nFirst: " + queryString1 + "\nSecond: " + queryString2);
            Assert.IsFalse(QueryString.Parse(queryString1.ToString()).Equals(queryString2), "After serializing and parsing expected, result changed.\n\nFirst: " + queryString1 + "\nSecond: " + queryString2);
            Assert.IsFalse(QueryString.Parse(queryString1.ToString()).Equals(QueryString.Parse(queryString2.ToString())), "After serializing and parsing both, result changed.\n\nFirst: " + queryString1 + "\nSecond: " + queryString2);
        }
    }
}
