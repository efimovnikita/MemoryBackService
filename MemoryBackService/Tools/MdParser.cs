using System;
using System.Collections.Generic;
using MemoryBackService.Models;
using Sprache;

namespace MemoryBackService.Tools
{
    public static class MdParser
    {
        private static readonly Parser<string> QuotedTextParser =
            from open in Parse.Char('"').Once()
            from text in Parse.CharExcept('"').Many().Text()
            from close in Parse.Char('"').Once()
            select text;

        public static readonly Parser<Title> TitleParser =
            from title in Parse.String("title")
            from separator in Parse.Char(':').Once()
            from whitespace in Parse.WhiteSpace.Many() 
            from text in QuotedTextParser
            select new Title { Text = text };
        
        public static readonly Parser<Category> CategoryParser =
            from title in Parse.String("category")
            from separator in Parse.Char(':').Once()
            from whitespace in Parse.WhiteSpace.Many() 
            from text in QuotedTextParser
            select new Category { Text = text };
        
        public static readonly Parser<Book> BookParser =
            from title in Parse.String("book")
            from separator in Parse.Char(':').Once()
            from whitespace in Parse.WhiteSpace.Many() 
            from text in QuotedTextParser
            select new Book { Name = text };

        public static readonly Parser<Tag> TagParser =
            from whitespaces in Parse.WhiteSpace.Many()
            from c in Parse.Char('-').Once()
            from whitespace in Parse.WhiteSpace.Once()
            from text in Parse.Letter.Many().Text()
            select new Tag { Text = text };

        public static readonly Parser<List<Tag>> ListTagsParser =
            from tags in TagParser.Many()
            select new List<Tag>(tags);

        public static readonly Parser<Tags> TagsParser =
            from tags in Parse.String("tags")
            from separator in Parse.Char(':').Once()
            from lineEnd in Parse.LineEnd
            from list in ListTagsParser
            select new Tags { List = list };
        
        public static readonly Parser<Date> DateParser =
            from title in Parse.String("date")
            from separator in Parse.Char(':').Once()
            from whitespace in Parse.WhiteSpace.Many() 
            from text in Parse.AnyChar.Until(Parse.LineTerminator).Text()
            select new Date { Value = DateTime.Parse(text)};
        
        public static readonly Parser<Layout> LayoutParser =
            from title in Parse.String("layout")
            from separator in Parse.Char(':').Once()
            from whitespace in Parse.WhiteSpace.Many() 
            from text in Parse.AnyChar.Until(Parse.LineTerminator).Text()
            select new Layout { Name = text};

        public static readonly Parser<Headline> HeadlineParser =
            from open in Parse.String("---").Until(Parse.LineTerminator).Token()
            from title in TitleParser.Token()
            from date in DateParser.Token()
            from layout in LayoutParser.Token()
            from category in CategoryParser.Token()
            from tags in TagsParser.Token()
            from book in BookParser.Token()
            from close in Parse.String("---").Until(Parse.LineTerminator).Token()
            select new Headline { Title = title, Date = date, Layout = layout, Category = category, Tags = tags, Book = book};

        public static readonly Parser<Md> WholeMdParser =
            from headline in HeadlineParser.Token()
            from text in Parse.AnyChar.Many().Text().End()
            select new Md {Headline = headline, Text = text};
    }
}