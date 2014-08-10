

namespace McGiv.TextKey.Tests
{
    using System;

    using NUnit.Framework;

    [TestFixture]
    public class IncludeTextProcessorTests
    {

        [Test]
        public void Value_With_No_Insert()
        {
            ITextRepository repo = new DictionaryTextRepository(new TextItem[]
                                                                     {
                                                                         new TextItem{Key = "abc",  Value = "abc abc"},
                                                                         new TextItem{Key = "def",  Value = "def $(abc) def"}
                                                                     });

            var processor = new IncludeTextProcessor(repo);


            var value = processor.Process("abc");

            Assert.AreEqual("abc abc", value);

        }

        [Test]
        public void Value_With_Single_Insert()
        {
            ITextRepository repo = new DictionaryTextRepository(new TextItem[]
                                                                     {
                                                                         new TextItem{Key = "abc",  Value = "abc abc"},
                                                                         new TextItem{Key = "def",  Value = "def $(abc) def"}
                                                                     });

            var processor = new IncludeTextProcessor(repo);



            var value = processor.Process("def");

            Assert.AreEqual("def abc abc def", value);
        }



        [Test]
        public void Value_With_Single_Insert_Ignore_Case()
        {
            ITextRepository repo = new DictionaryTextRepository(new TextItem[]
                                                                     {
                                                                         new TextItem{Key = "abc",  Value = "abc abc"},
                                                                         new TextItem{Key = "def",  Value = "def $(aBc) def"}
                                                                     });

            var processor = new IncludeTextProcessor(repo);



            var value = processor.Process("def");

            Assert.AreEqual("def abc abc def", value);
        }


        [Test]
        public void Value_With_Multiple_Inserts()
        {
            ITextRepository repo = new DictionaryTextRepository(new TextItem[]
                                                                     {
                                                                         new TextItem{Key = "abc",  Value = "abc abc"},
                                                                         new TextItem{Key = "def",  Value = "def $(abc) def $(abc) def"}
                                                                     });

            var processor = new IncludeTextProcessor(repo);


            var value = processor.Process("def");

            Assert.AreEqual("def abc abc def abc abc def", value);
        }

        [Test]
        public void Value_With_Multiple_Level_Insert()
        {
            ITextRepository repo = new DictionaryTextRepository(new TextItem[]
                                                                     {
                                                                         new TextItem{Key = "abc",  Value = "abc abc"},
                                                                         new TextItem{Key = "def",  Value = "def $(abc) def"},
                                                                         new TextItem{Key = "hij",  Value = "hij $(def) hij"}
                                                                     });

            var processor = new IncludeTextProcessor(repo);



            var value = processor.Process("hij");

            Assert.AreEqual("hij def abc abc def hij", value);
        }


        [Test]
        public void Circular_Reference_Keys_Throws_Exception()
        {
            ITextRepository repo = new DictionaryTextRepository(new TextItem[]
                                                                     {
                                                                         new TextItem{Key = "abc",  Value = "abc $(def) abc"},
                                                                         new TextItem{Key = "def",  Value = "def $(abc) def"}
                                                                     });

            var processor = new IncludeTextProcessor(repo);


            var ex = Assert.Throws<InvalidOperationException>(() => processor.Process("def"));

            Console.Write(ex.Message);
            
        }


        [Test]
        public void Circular_Reference_Keys_Ignore_Case_Throws_Exception()
        {
            ITextRepository repo = new DictionaryTextRepository(new TextItem[]
                                                                     {
                                                                         new TextItem{Key = "aBc",  Value = "abc $(def) abc"},
                                                                         new TextItem{Key = "def",  Value = "def $(abc) def"}
                                                                     });

            var processor = new IncludeTextProcessor(repo);


            var ex = Assert.Throws<InvalidOperationException>(() => processor.Process("def"));

            Console.Write(ex.Message);

        }


        [Test]
        public void Value_With_Insert_First_Character()
        {
            ITextRepository repo = new DictionaryTextRepository(new TextItem[]
                                                                     {
                                                                         new TextItem{Key = "abc",  Value = "abc"},
                                                                         new TextItem{Key = "def",  Value = "$(abc)xxx"}
                                                                     });

            var processor = new IncludeTextProcessor(repo);



            var value = processor.Process("def");

            Assert.AreEqual("abcxxx", value);
        }



        [Test]
        public void Value_With_Insert_Last_Character()
        {
            ITextRepository repo = new DictionaryTextRepository(new TextItem[]
                                                                     {
                                                                         new TextItem{Key = "abc",  Value = "abc"},
                                                                         new TextItem{Key = "def",  Value = "xxx$(abc)"}
                                                                     });

            var processor = new IncludeTextProcessor(repo);



            var value = processor.Process("def");

            Assert.AreEqual("xxxabc", value);
        }


        [Test]
        public void Value_With_Insert_First_Last_Character()
        {
            ITextRepository repo = new DictionaryTextRepository(new TextItem[]
                                                                     {
                                                                         new TextItem{Key = "abc",  Value = "abc"},
                                                                         new TextItem{Key = "def",  Value = "$(abc)"}
                                                                     });

            var processor = new IncludeTextProcessor(repo);



            var value = processor.Process("def");

            Assert.AreEqual("abc", value);
        }



        [Test]
        public void Value_With_Multiple_Inserts_First_Last_Character()
        {
            ITextRepository repo = new DictionaryTextRepository(new TextItem[]
                                                                     {
                                                                         new TextItem{Key = "abc",  Value = "abc"},
                                                                         new TextItem{Key = "def",  Value = "def"},
                                                                         new TextItem{Key = "hij",  Value = "$(abc)$(def)"},
                                                                     });

            var processor = new IncludeTextProcessor(repo);



            var value = processor.Process("hij");

            Assert.AreEqual("abcdef", value);
        }



        /// <summary>
        /// When the tail of an insert is mieeing treat the rest of the insert as normal text.
        /// </summary>
        [Test]
        public void Invalid_Key_Format_Missing_Tail()
        {
            ITextRepository repo = new DictionaryTextRepository(new TextItem[]
                                                                     {
                                                                         new TextItem{Key = "abc",  Value = "123"},
                                                                         new TextItem{Key = "def",  Value = "def"},
                                                                         new TextItem{Key = "hij",  Value = "$(abc$(def)"}
                                                                     });

            var processor = new IncludeTextProcessor(repo);



            var value = processor.Process("hij");

            Assert.AreEqual("$(abcdef", value, "Failed to find 'hij'");


        }



        /// <summary>
        /// When only the head of an insert is given treat as normal text.
        /// </summary>
        [Test]
        public void Invalid_Key_Format_Missing_Key_And_Tail()
        {
            ITextRepository repo = new DictionaryTextRepository(new TextItem[]
                                                                     {
                                                                         new TextItem{Key = "abc",  Value = "123"},
                                                                         new TextItem{Key = "def",  Value = "def"},
                                                                         new TextItem{Key = "hij",  Value = "$($(def)"},
                                                                         new TextItem{Key = "klm",  Value = "$("},
                                                                         new TextItem{Key = "nop",  Value = "$(sdfsdf"},
                                                                     });

            var processor = new IncludeTextProcessor(repo);



            var value = processor.Process("hij");

            Assert.AreEqual("$(def", value, "Failed to find 'hij'");


            value = processor.Process("klm");

            Assert.AreEqual("$(", value, "Failed to find 'klm'");

            value = processor.Process("nop");

            Assert.AreEqual("$(sdfsdf", value, "Failed to find 'nop'");
        }




        /// <summary>
        /// When the head of the insert is missing treat the rest of the insert as normal text.
        /// </summary>
        [Test]
        public void Invalid_Key_Format_Missing_Head()
        {
            ITextRepository repo = new DictionaryTextRepository(new TextItem[]
                                                                     {
                                                                         new TextItem{Key = "abc",  Value = "123"},
                                                                         new TextItem{Key = "def",  Value = "def"},
                                                                         new TextItem{Key = "hij",  Value = "(abc)$(def)"},
                                                                     });

            var processor = new IncludeTextProcessor(repo);



            var value = processor.Process("hij");

            Assert.AreEqual("(abc)def", value);
        }



        /// <summary>
        /// When an insert's key is empty treat as notmal text.
        /// </summary>
        [Test]
        public void Invalid_Key_Format_Empty()
        {
            ITextRepository repo = new DictionaryTextRepository(new TextItem[]
                                                                     {
                                                                         new TextItem{Key = "abc",  Value = "123"},
                                                                         new TextItem{Key = "def",  Value = "def"},
                                                                         new TextItem{Key = "hij",  Value = "$()$(def)"},
                                                                     });

            var processor = new IncludeTextProcessor(repo);



            var value = processor.Process("hij");

            Assert.AreEqual("$()def", value);
        }




        /// <summary>
        /// When a key is not found in repo 'null' should be returned. Treat any insert with a null value as normal text.
        /// </summary>
        [Test]
        public void Invalid_Key_Not_Found()
        {
            ITextRepository repo = new DictionaryTextRepository(new TextItem[]
                                                                     {
                                                                         new TextItem{Key = "abc",  Value = "123"},
                                                                         new TextItem{Key = "def",  Value = "def"},
                                                                         new TextItem{Key = "hij",  Value = "$(xxx)$(def)"},
                                                                     });

            var processor = new IncludeTextProcessor(repo);



            var value = processor.Process("hij");

            Assert.AreEqual("$(xxx)def", value);
        }
    }
}
