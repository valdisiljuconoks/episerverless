using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAnnotations;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Web;

namespace Web1.Models.Media
{
    [ContentType(GUID = "0A89E464-56D4-449F-AEA8-2BF774AB8730")]
    [MediaDescriptor(ExtensionString = "jpg,jpeg,jpe,ico,gif,bmp,png")]
    public class ImageFile : ImageData
    {
        /// <summary>
        ///     Gets or sets the copyright.
        /// </summary>
        /// <value>
        ///     The copyright.
        /// </value>
        public virtual string Copyright { get; set; }

        [UIHint(UIHint.Textarea)]
        public virtual string AsciiArt { get; set; }

        public virtual string Description { get; set; }

        public virtual string Tags { get; set; }
    }
}
