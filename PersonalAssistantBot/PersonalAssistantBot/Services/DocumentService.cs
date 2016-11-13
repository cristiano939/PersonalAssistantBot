using Lime.Messaging.Contents;
using Lime.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public DocumentCollection CreateCarrossel()
        {
            var docCollection = new DocumentCollection();
            docCollection.Items = new DocumentSelect[];
            docCollection.ItemType = DocumentSelect.MediaType;
        }
    }
}
