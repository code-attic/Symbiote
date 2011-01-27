/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System.Collections.Generic;

namespace Symbiote.Http
{
    public class ContentType
    {
        public static readonly string AtomFeed = "application/atom+xml";
        public static readonly string Json = "application/json";
        public static readonly string Javascript = "application/javascript";
        public static readonly string Binary = "application/octet-stream";
        public static readonly string Pdf = "application/pdf";
        public static readonly string Soap = "application/soap+xml";
        public static readonly string Xhtml = "application/xhtml+xml";
        public static readonly string Zip = "application/zip";
        public static readonly string Mp4 = "audio/mp4";
        public static readonly string Mp3 = "audio/mpeg";
        public static readonly string Wma = "audio/x-ms-wma";
        public static readonly string Bmp = "image/bmp";
        public static readonly string Gif = "image/gif";
        public static readonly string Jpeg = "image/jpeg";
        public static readonly string Png = "image/png";
        public static readonly string Svg = "image/svg+xml";
        public static readonly string Tiff = "image/tiff";
        public static readonly string Ico = "image/vnd.microsoft.icon";
        public static readonly string Message = "message/http";
        public static readonly string Mixed = "multipart/mixed";
        public static readonly string Form = "multipart/related";
        public static readonly string Signed = "multipart/signed";
        public static readonly string Encrypted = "multipart/encrypted";
        public static readonly string Css = "text/css";
        public static readonly string Csv = "text/csv";
        public static readonly string Html = "text/html";
        public static readonly string Plain = "text/plain";
        public static readonly string Xml = "text/xml";
        public static readonly string Mpeg = "video/mpeg";
        public static readonly string Mp4Video = "video/mp4";
        public static readonly string QuickTime = "video/quicktime";
        public static readonly string WebM = "video/WebM";
        public static readonly string Wmv = "video/x-ms-mv";
        public static readonly string OpenDoc = "application/vnd.oasis.opendocument.text";
        public static readonly string OpenSpreadSheet = "application/vnd.oasis.opendocument.spreadsheet";
        public static readonly string OpenPresentation = "application/vnd.oasis.opendocument.presentation";
        public static readonly string Excel = "application/vnd.ms-excel";
        public static readonly string Word = "application/msword";
        public static readonly string PowerPoint = "application/vnd.ms-powerpoint";
        public static readonly string DocX = "application/vnd.openxmlformats-officedocument.wordpressingml.document";
        public static readonly string ExcelX = "application/vnd.openformats-officedocument.spreadsheetml.sheet";
        public static readonly string PPX = "application/vnd.openformats-officedocument.presentationml.presentation";
        public static readonly string Tar = "application/x-tar";
        public static readonly string Rar = "application/x-rar-compressed";
        public static readonly string Flash = "application/x-shockwave-flash";
        public static readonly string JqTemplate = "text/x-jquery-tmpl";
        public static readonly string Pfx = "application/x-pkcs12";
        public static readonly string P7b = "application/x-pkcs7-certificates";
        public static readonly string P7r = "application/x-pkcs7-certreqrsp";
        public static readonly string P7c = "application/x-pkcs7-mime";
        public static readonly string P7s = "application/x-pkcs7-signature";

        public static readonly Dictionary<string, string> ContentByExtension = new Dictionary<string, string>()
        {
            {"bmp", Bmp},
            {"css", Css},
            {"csv", Csv},
            {"doc", Word},
            {"docx", DocX},
            {"gif", Gif},
            {"htm", Html},
            {"html", Html},
            {"ico", Ico},
            {"jpg", Jpeg},
            {"js",Javascript},
            {"mp3", Mp3},
            {"mp4", Mp4},
            {"mpg", Mpeg},
            {"pdf", Pdf},
            {"png", Png},
            {"ppt", PowerPoint},
            {"ppx", PPX},
            {"qt", QuickTime},
            {"rar", Rar},
            {"svg", Svg},
            {"tar", Tar},
            {"tff", Tiff},
            {"txt", Plain},
            {"wma", Wma},
            {"wmv", Wmv},
            {"xhtml", Xhtml},
            {"xml", Xml},
            {"xls", Excel},
            {"xlsx", ExcelX},
            {"zip",Zip},
        };
    }
}
