﻿using Lime.Messaging.Contents;
using Lime.Protocol;
using PersonalAssistantBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PersonalAssistantBot.Services
{
    public class DocumentService
    {

        public MediaLink CreateMediaLink(string title, string text, Uri url, string mediatype)
        {
            var type = mediatype.Split('/')[0];
            var subtype = mediatype.Split('/')[1];
            MediaLink mediaLink = new MediaLink
            {
                Title = title,
                Text = text,
                PreviewUri = url,
                Uri = url,
                Size = 1,
                Type = new MediaType(type, subtype)

            };
            return mediaLink;
        }

        public DocumentSelectOption[] CreateDocumentSelectOptions(List<string> options)
        {
            DocumentSelectOption[] opts = new DocumentSelectOption[options.Count];
            int i = 0;
            foreach (var option in options)
            {
                opts[i] = new DocumentSelectOption();
                opts[i].Label = new DocumentContainer { Value = new PlainText { Text = option } };
                opts[i].Value = new DocumentContainer { Value = new PlainText { Text = option } };
                i++;
            }
            return opts;

        }

        public WebLink CreateWebLink(string title, string text, Uri url, string mediatype)
        {
            WebLink webLink = new WebLink
            {
                Title = title,
                Text = text,
                Uri = url,
                PreviewUri = url,
                PreviewType = MediaType.Parse(mediatype)
            };
            return webLink;
        }

        public DocumentCollection CreateCarrossel(List<CarrosselCard> cards)
        {
            var docCollection = new DocumentCollection();
            docCollection.Items = new DocumentSelect[cards.Count];
            docCollection.ItemType = DocumentSelect.MediaType;
            int i = 0;
            foreach (var card in cards)
            {
                var doc = new DocumentSelect();
                doc.Header = new DocumentContainer();
                doc.Header.Value = card.CardMediaHeader;
                doc.Options = CreateDocumentSelectOptions(card.options);
                docCollection.Items[i] = doc;
                i++;
            }

            return docCollection;

        }




    }
}
