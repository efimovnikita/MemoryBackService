using System;
using System.Collections.Generic;
using MemoryBackService.Models;
using MemoryBackService.Tools;
using NUnit.Framework;
using Sprache;

namespace MemoryBackService.Tests
{
    public class Tests
    {
        [Test]
        public void ShouldParseTitle()
        {
            const string text = "Механизм действия цианида";
            string input = $"title: \"{text}\"";
            Assert.AreEqual(new Title { Text = text }.Text, 
                MdParser.TitleParser.Parse(input).Text);
            
            string input2 = $"title:\"{text}\"";
            Assert.AreEqual(new Title{ Text = text }.Text, 
                MdParser.TitleParser.Parse(input2).Text);
        }
        
        [Test]
        public void ShouldParseCategory()
        {
            const string text = "моллекулярная биология";
            string input = $"category: \"{text}\"";
            Assert.AreEqual(new Category{Text = text}.Text, 
                MdParser.CategoryParser.Parse(input).Text);
        }
        
        [Test]
        public void ShouldParseBook()
        {
            const string text = "Брюс Альбертс. Основы молекулярной биологии клетки";
            string input = $"book: \"{text}\"";
            Assert.AreEqual(new Book{Name = text}.Name, 
                MdParser.BookParser.Parse(input).Name);
        }
        
        [Test]
        public void ShouldParseDate()
        {
            const string text = "2017-05-13";
            string input = $"date: {text}";
            Assert.AreEqual(new Date{Value = DateTime.Parse(text)}.Value, 
                MdParser.DateParser.Parse(input).Value);
        }
        
        [Test]
        public void ShouldParseLayout()
        {
            const string text = "post.njk";
            string input = $"layout: {text}";
            Assert.AreEqual(new Layout{Name = text}.Name, 
                MdParser.LayoutParser.Parse(input).Name);
        }

        [Test]
        public void ShouldParseTag()
        {
            const string text = "posts";
            string input = $"  - {text}";
            Assert.AreEqual(new Tag{Text = text}.Text, 
                MdParser.TagParser.Parse(input).Text);
        }
        
        [Test]
        public void ShouldParseListOfTags()
        {
            const string input = @"  - posts
              - molBiol
              - alberts";
            List<Tag> expectedList = new() {new Tag{Text = "posts"}, new Tag{Text = "molBiol"}, new Tag{Text = "alberts"}};
            Assert.AreEqual(expectedList.Count, 
                MdParser.ListTagsParser.Parse(input).Count);
            
            Assert.AreEqual(expectedList[0].Text, "posts");
            Assert.AreEqual(expectedList[1].Text, "molBiol");
            Assert.AreEqual(expectedList[2].Text, "alberts");
        }
        
        [Test]
        public void ShouldParseTags()
        {
            const string input = @"tags:
              - posts
              - molBiol
              - alberts";
            List<Tag> expectedList = new() {new Tag{Text = "posts"}, new Tag{Text = "molBiol"}, new Tag{Text = "alberts"}};
            Tags tags = new Tags{List = expectedList};
            Assert.AreEqual(tags.List.Count, 
                MdParser.TagsParser.Parse(input).List.Count);
            
            Assert.AreEqual(tags.List[1].Text, "molBiol");
            Assert.AreEqual(tags.List[2].Text, "alberts");
        }
        
        [Test]
        public void ShouldParseHeadLine()
        {
            const string text = @"---
            title: ""Механизм действия цианида""
            date: 2017-05-13
            layout: post.njk
            category: ""моллекулярная биология""
            tags:
                - posts
                - molBiol
                - alberts
            book: ""Брюс Альбертс. Основы молекулярной биологии клетки""
                ---";

            Headline headline = MdParser.HeadlineParser.Parse(text);
            Assert.AreEqual(3, headline.Tags.List.Count);
            Assert.AreEqual("Брюс Альбертс. Основы молекулярной биологии клетки", headline.Book.Name);
        }
        
        [Test]
        public void ShouldParseWholeMd()
        {
            const string text = @"---
            title: ""Механизм действия цианида""
            date: 2017-05-13
            layout: post.njk
            category: ""моллекулярная биология""
            tags:
                - posts
                - molBiol
                - alberts
            book: ""Брюс Альбертс. Основы молекулярной биологии клетки""
                ---

            Биосинтетические ферменты часто проводят энергетически невыгодные реакции, сопрягая их с энергетически выгодным гидролизом АТФ. Пул АТФ из-за этого расходуется на поддержание клеточных процессов так же, как аккумулятор может питать электрический двигатель. Если активность митохондрий приостановить, уровень АТФ упадет, и клеточная батарея будет разряжаться; в итоге, энергетически невыгодные реакции больше не смогут идти, и клетка умрет. Яд цианид, блокирующий транспорт электронов во внутренней мембране митохондрий, вызывает смерть именно таким образом.";

            Md md = MdParser.WholeMdParser.Parse(text);
            Assert.AreEqual(3, md.Headline.Tags.List.Count);
            Assert.AreEqual("Брюс Альбертс. Основы молекулярной биологии клетки", md.Headline.Book.Name);
            Assert.AreEqual("Биосинтетические ферменты часто проводят энергетически невыгодные реакции, сопрягая их с энергетически выгодным гидролизом АТФ. Пул АТФ из-за этого расходуется на поддержание клеточных процессов так же, как аккумулятор может питать электрический двигатель. Если активность митохондрий приостановить, уровень АТФ упадет, и клеточная батарея будет разряжаться; в итоге, энергетически невыгодные реакции больше не смогут идти, и клетка умрет. Яд цианид, блокирующий транспорт электронов во внутренней мембране митохондрий, вызывает смерть именно таким образом.", md.Text);
        }
        
        [Test]
        public void ShouldParseAnotherWholeMd()
        {
            const string text = @"---
            title: ""Механизмы накопления агрессии""
            date: 2016-04-17
            layout: post.njk
            category: ""этология""
            tags:
                - posts
            book: ""Виктор Дольник. Непослушное дитя биосферы""
                    ---

                Механизм накопления агрессии взрывает изнутри маленькие замкнутые коллективы людей. На зимовку или в экспедицию выезжают несколько дружных, уважающих друг друга человек, твердо знающих, что в таких условиях конфликтовать нельзя. Проходит время, и, если нет внешнего объекта для проявления агрессивности, люди в группе начинают ненавидеть друг друга, и долго сдерживавшаяся агрессивность в конце концов находит самый пустяковый повод для большого скандала.

                Известно много случаев, когда попавшие в такой эксперимент близкие друзья доходили до бессмысленного убийства. В обычной жизни наша агрессивность ежедневно разряжается через массу незначительных конфликтов с окружающими. Мы можем научиться кое-как управлять своей агрессивностью, но полностью устранить ее мы не можем, ведь это один из сильнейших инстинктов человека. И важно помнить, что, ограждая агрессивную личность от раздражителей, мы не снижаем ее агрессивность, а только накапливаем. Она все равно прорвется, причем сразу большой порцией.";

            Md md = MdParser.WholeMdParser.Parse(text);
            Assert.AreEqual(1, md.Headline.Tags.List.Count);
        }
    }
}