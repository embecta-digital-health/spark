using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using System.Xml.Xsl;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Spark.Engine.Core;
using Spark.Engine.Extensions;

// using Spark.Service;

namespace Spark.Engine.Formatters.Deprecated
{
    public class HtmlFhirFormatter : FhirMediaTypeFormatter
    {
        public HtmlFhirFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            return Task.Factory.StartNew<object>(() =>
            {
                try
                {
                    throw new NotSupportedException(string.Format("Cannot read unsupported type {0} from body", type.Name));
                }
                catch (FormatException exc)
                {
                    throw Error.BadRequest("Body parsing failed: " + exc.Message);
                }
            });
        }

        public override void SetDefaultContentHeaders(Type type, HttpContentHeaders headers, MediaTypeHeaderValue mediaType)
        {
            base.SetDefaultContentHeaders(type, headers, mediaType);
            headers.ContentType = new MediaTypeHeaderValue("text/html");
        }

        private void WriteHTMLOutput(Type type, object value, Stream writeStream)
        {
            StreamWriter writer = new StreamWriter(writeStream, Encoding.UTF8);
            writer.WriteLine("<html>");
            writer.WriteLine("<head>");
            writer.WriteLine("  <link href=\"/Content/fhir-html.css\" rel=\"stylesheet\"></link>");
            writer.WriteLine("</head>");
            writer.WriteLine("<body>");
            if (type == typeof(OperationOutcome))
            {
                OperationOutcome oo = (OperationOutcome) value;

                if (oo.Text != null)
                    writer.Write(oo.Text.Div);
            }
            else if (type == typeof(Resource))
            {
                if (value is Bundle)
                {
                    Bundle resource = (Bundle) value;

                    if (resource.SelfLink != null)
                    {
                        writer.WriteLine(string.Format("Searching: {0}<br/>", resource.SelfLink.OriginalString));

                        // Hl7.Fhir.Model.Parameters query = FhirParser.ParseQueryFromUriParameters(collection, parameters);

                        NameValueCollection ps = resource.SelfLink.ParseQueryString();
                        if (ps.AllKeys.Contains(FhirParameter.SORT))
                            writer.WriteLine(string.Format("    Sort by: {0}<br/>", ps[FhirParameter.SORT]));
                        if (ps.AllKeys.Contains(FhirParameter.SUMMARY))
                            writer.WriteLine("    Summary only<br/>");
                        if (ps.AllKeys.Contains(FhirParameter.COUNT))
                            writer.WriteLine(string.Format("    Count: {0}<br/>", ps[FhirParameter.COUNT]));
                        if (ps.AllKeys.Contains(FhirParameter.SNAPSHOT_INDEX))
                            writer.WriteLine(string.Format("    From RowNum: {0}<br/>", ps[FhirParameter.SNAPSHOT_INDEX]));
                        if (ps.AllKeys.Contains(FhirParameter.SINCE))
                            writer.WriteLine(string.Format("    Since: {0}<br/>", ps[FhirParameter.SINCE]));


                        foreach (var item in ps.AllKeys.Where(k => !k.StartsWith("_")))
                        {
                            if (ModelInfo.SearchParameters.Exists(s => s.Name == item))
                            {
                                writer.WriteLine(string.Format("    {0}: {1}<br/>", item, ps[item]));
                                //var parameters = transportContext..Request.TupledParameters();
                                //int pagesize = Request.GetIntParameter(FhirParameter.COUNT) ?? Const.DEFAULT_PAGE_SIZE;
                                //bool summary = Request.GetBooleanParameter(FhirParameter.SUMMARY) ?? false;
                                //string sortby = Request.GetParameter(FhirParameter.SORT);
                            }
                            else
                            {
                                writer.WriteLine(string.Format("    <i>{0}: {1} (excluded)</i><br/>", item, ps[item]));
                            }
                        }
                    }

                    if (resource.FirstLink != null)
                        writer.WriteLine(string.Format("First Link: {0}<br/>", resource.FirstLink.OriginalString));
                    if (resource.PreviousLink != null)
                        writer.WriteLine(string.Format("Previous Link: {0}<br/>", resource.PreviousLink.OriginalString));
                    if (resource.NextLink != null)
                        writer.WriteLine(string.Format("Next Link: {0}<br/>", resource.NextLink.OriginalString));
                    if (resource.LastLink != null)
                        writer.WriteLine(string.Format("Last Link: {0}<br/>", resource.LastLink.OriginalString));

                    // Write the other Bundle Header data
                    writer.WriteLine(string.Format("<span style=\"word-wrap: break-word; display:block;\">Type: {0}, {1} of {2}</span>", resource.Type.ToString(), resource.Entry.Count, resource.Total));

                    foreach (var item in resource.Entry)
                    {
                        //IKey key = item.ExtractKey();

                        writer.WriteLine("<div class=\"item-tile\">");
                        if (item.IsDeleted())
                        {
                            if (item.Request != null)
                            {
                                string id = item.Request.Url;
                                writer.WriteLine(string.Format("<span style=\"word-wrap: break-word; display:block;\">{0}</span>", id));
                            }

                            //if (item.Deleted.Instant.HasValue)
                            //    writer.WriteLine(String.Format("<i>Deleted: {0}</i><br/>", item.Deleted.Instant.Value.ToString()));

                            writer.WriteLine("<hr/>");
                            writer.WriteLine("<b>DELETED</b><br/>");
                        }
                        else if (item.Resource != null)
                        {
                            Key key = item.Resource.ExtractKey();
                            string visualurl = key.WithoutBase().ToUriString();
                            string realurl = key.ToUriString() + "?_format=html";

                            writer.WriteLine(string.Format("<a style=\"word-wrap: break-word; display:block;\" href=\"{0}\">{1}</a>", realurl, visualurl));
                            if (item.Resource.Meta != null && item.Resource.Meta.LastUpdated.HasValue)
                                writer.WriteLine(string.Format("<i>Modified: {0}</i><br/>", item.Resource.Meta.LastUpdated.Value.ToString()));
                            writer.WriteLine("<hr/>");

                            if (item.Resource is DomainResource)
                            {
                                if ((item.Resource as DomainResource).Text != null && !string.IsNullOrEmpty((item.Resource as DomainResource).Text.Div))
                                    writer.Write((item.Resource as DomainResource).Text.Div);
                                else
                                    writer.WriteLine(string.Format("Blank Text: {0}<br/>", item.Resource.ExtractKey().ToUriString()));
                            }
                            else
                            {
                                writer.WriteLine("This is not a domain resource");
                            }
                        }
                        writer.WriteLine("</div>");
                    }
                }
                else
                {
                    DomainResource resource = (DomainResource) value;
                    string org = resource.ResourceBase + "/" + resource.ResourceType + "/" + resource.Id;
                    // TODO: This is probably a bug in the service (Id is null can throw ResourceIdentity == null
                    // reference ResourceIdentity : org = resource.ResourceIdentity().OriginalString;
                    writer.WriteLine(string.Format("Retrieved: {0}<hr/>", org));

                    string text = (resource.Text != null) ? resource.Text.Div : null;
                    writer.Write(text);
                    writer.WriteLine("<hr/>");

                    SummaryType summary = requestMessage.RequestSummary();
                    string xml = FhirSerializer.SerializeResourceToXml(resource, summary);
                    XPathDocument xmlDoc = new XPathDocument(new StringReader(xml));

                    // And we also need an output writer
                    TextWriter output = new StringWriter(new StringBuilder());

                    // Now for a little magic
                    // Create XML Reader with style-sheet
                    //disabled 2017-05-18 due to .net core 2.0 conversion
//                    System.Xml.XmlReader stylesheetReader = System.Xml.XmlReader.Create(new StringReader(Resources.RenderXMLasHTML));

                    XslCompiledTransform xslTransform = new XslCompiledTransform();
                    //disabled 2017-05-18 due to .net core 2.0 conversion
                    //                    xslTransform.Load(stylesheetReader);
                    xslTransform.Transform(xmlDoc, null, output);

                    writer.WriteLine(output.ToString());
                }
            }

            writer.WriteLine("</body>");
            writer.WriteLine("</html>");
            writer.Flush();
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            return Task.Factory.StartNew(() => { WriteHTMLOutput(type, value, writeStream); });
        }
    }
}