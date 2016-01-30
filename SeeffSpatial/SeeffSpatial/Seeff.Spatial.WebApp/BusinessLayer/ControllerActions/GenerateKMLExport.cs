using Seeff.Spatial.WebApp.BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml.Linq;

namespace Seeff.Spatial.WebApp.BusinessLayer.ControllerActions
{
    public partial class ControllerActions
    {
        public static ExportLicenseModelResult GenerateKMLExport(ExportLicenseModel exportModel)
        {
            SeeffLicense targetLicense = GlobalAreaCache.Instance.SeeffLicenses.First(lic => lic.LicenseID == exportModel.LicenseID);

            XNamespace xmlns = "http://www.opengis.net/kml/2.2";
            XDocument documentExport = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
            XElement documentContainer = new XElement("Document", new XElement("name", "Seeff License Export: " + targetLicense.LicenseName));
            XElement root = new XElement(xmlns + "kml", documentContainer);
            documentExport.Add(root);

            XElement licensePolyElement = CreatePolyExportElement(targetLicense);
            documentContainer.Add(licensePolyElement);

            if (exportModel.IncludeSuburbs)
            {
                foreach (var suburb in targetLicense.Suburbs)
                {
                    XElement suburbPolyElement = CreatePolyExportElement(suburb);
                    documentContainer.Add(suburbPolyElement);
                }
            }

            if (exportModel.IncludeOrphanedRecords)
            {
                var orphanedRecords = GetOrphanedProperties(targetLicense);
                foreach (var orphan in orphanedRecords.Orphans)
                {
                    XElement orphanElement = CreateOrphanExportElement(orphan);
                    documentContainer.Add(orphanElement);
                }
            }

            // Add styles
            foreach (var styleItem in CreateMarkerStyles())
            {
                documentContainer.Add(styleItem);
            }

            foreach (var styleItem in CreatePolyStyles())
            {
                documentContainer.Add(styleItem);
            }

            string docString = documentExport.ToString();
            using (StreamWriter stream = new StreamWriter(HttpContext.Current.Server.MapPath("~/KMLExports/" + targetLicense.LicenseName + ".kml"), false))
            {
                stream.Write(docString); 
            }
            return new ExportLicenseModelResult
            {
                Successful = true,
                 SeeffLicense = targetLicense
            };
        }

        private static XElement CreatePolyExportElement(SpatialModelBase spatialObject)
        {
            string name = String.Empty;
            string styleURL = string.Empty;
            string licensePosition = string.Empty; 

            var license = spatialObject as SeeffLicense;
            if (license != null)
            {
                name = SecurityElement.Escape(license.LicenseName);
                styleURL = "#licenseStyle";        
                licensePosition = string.Format(@"<LookAt>
                                        <longitude>{0}</longitude>
                                        <latitude>{1}</latitude>
                                        <altitude>2000</altitude>
                                        <range>20000</range>
                                       </LookAt>", license.Centroid.Longitude.Value.ToString(CultureInfo.InvariantCulture), license.Centroid.Latitude.Value.ToString(CultureInfo.InvariantCulture));        
            }
            var suburb = spatialObject as SeeffSuburb;
            if (suburb != null)
            {
                name = SecurityElement.Escape(suburb.AreaName) + " (" + suburb.SeeffAreaID + ")";
                styleURL = "#suburbStyle";
            }

            string[] coordinateSets = spatialObject.PolyWKT.Replace("POLYGON ((", "").Replace("))", "").Split(new[] { ',' });
            List<string> coordinateStringList = new List<string>();
            foreach (var item in coordinateSets)
            {
                string[] latLngPair = item.Trim().Split(new[] { ' ' });
                string lng = latLngPair[0];
                string lat = latLngPair[1];
                string outputPair = lng + "," + lat + ",0.0";

                coordinateStringList.Add(outputPair);
            }
            string output = coordinateStringList.Aggregate((s1,s2) => s1 + " " + s2);

            string content = string.Format(@"<Placemark>
			                    <name>{0}</name>
                                {1}
			                    <styleUrl>{2}</styleUrl>
			                    <ExtendedData>
			                    </ExtendedData>
			                    <Polygon>
				                    <outerBoundaryIs>
					                    <LinearRing>
						                    <tessellate>1</tessellate>
						                    <coordinates>{3}</coordinates>
					                    </LinearRing>
				                    </outerBoundaryIs>
			                    </Polygon>
		                    </Placemark>", name, licensePosition, styleURL, output);

            XElement result = XElement.Parse(content);
            return result;
        }

        private static XElement CreateOrphanExportElement(OrphanedProperty orphan)
        {
            string coordinates = orphan.LatLng.Lng.ToString(CultureInfo.InvariantCulture) + "," + orphan.LatLng.Lat.ToString(CultureInfo.InvariantCulture) + ",0.0";

            string content = string.Format(@"<Placemark>
			                                    <name>{0}</name>
			                                    <styleUrl>{1}</styleUrl>
			                                    <ExtendedData>
			                                    </ExtendedData>
			                                    <Point>
				                                    <coordinates>{2}</coordinates>
			                                    </Point>
		                                    </Placemark>", orphan.LightstonePropertyID, "#markerStyle", coordinates);

            XElement result = XElement.Parse(content);
            return result;
        }

        private static List<XElement> CreateMarkerStyles()
        {
            XElement msn = XElement.Parse(@"<Style id='markerStyle-normal'>
			                    <IconStyle>
				                    <color>ff3644DB</color>
				                    <scale>1.1</scale>
				                    <Icon>
					                    <href>http://www.gstatic.com/mapspro/images/stock/503-wht-blank_maps.png</href>
				                    </Icon>
				                    <hotSpot x='16' y='31' xunits='pixels' yunits='insetPixels'>
				                    </hotSpot>
			                    </IconStyle>
			                    <LabelStyle>
				                    <scale>0.0</scale>
			                    </LabelStyle>
			                    <BalloonStyle>
				                    <text><![CDATA[<h3>$[name]</h3>]]></text>
			                    </BalloonStyle>
		                    </Style>");

            XElement msh = XElement.Parse(@"<Style id='markerStyle-highlight'>
			                    <IconStyle>
				                    <color>ff3644DB</color>
				                    <scale>1.1</scale>
				                    <Icon>
					                    <href>http://www.gstatic.com/mapspro/images/stock/503-wht-blank_maps.png</href>
				                    </Icon>
				                    <hotSpot x='16' y='31' xunits='pixels' yunits='insetPixels'>
				                    </hotSpot>
			                    </IconStyle>
			                    <LabelStyle>
				                    <scale>1.1</scale>
			                    </LabelStyle>
			                    <BalloonStyle>
				                    <text><![CDATA[<h3>$[name]</h3>]]></text>
			                    </BalloonStyle>
		                    </Style>");

            XElement sm = XElement.Parse(@"<StyleMap id='markerStyle'>
			                    <Pair>
				                    <key>normal</key>
				                    <styleUrl>#markerStyle-normal</styleUrl>
			                    </Pair>
			                    <Pair>
				                    <key>highlight</key>
				                    <styleUrl>#markerStyle-highlight</styleUrl>
			                    </Pair>
		                    </StyleMap>");

            return new List<XElement>(new XElement[] { msn, msh, sm });
        }

        private static List<XElement> CreatePolyStyles()
        {
            XElement lsn = XElement.Parse(@"<Style id='licenseStyle-normal'>
			                    <LineStyle>
				                    <color>ffF08641</color>
				                    <width>2</width>
			                    </LineStyle>
			                    <PolyStyle>
				                    <color>6DF08641</color>
				                    <fill>1</fill>
				                    <outline>1</outline>
			                    </PolyStyle>
			                    <BalloonStyle>
				                    <text><![CDATA[<h3>$[name]</h3>]]></text>
			                    </BalloonStyle>
		                    </Style>");

            XElement lsh = XElement.Parse(@"<Style id='licenseStyle-highlight'>
			                    <LineStyle>
				                    <color>ffF08641</color>
				                    <width>3.0</width>
			                    </LineStyle>
			                    <PolyStyle>
				                    <color>6DF08641</color>
				                    <fill>1</fill>
				                    <outline>1</outline>
			                    </PolyStyle>
			                    <BalloonStyle>
				                    <text><![CDATA[<h3>$[name]</h3>]]></text>
			                    </BalloonStyle>
		                    </Style>");

            XElement ls = XElement.Parse(@"<StyleMap id='licenseStyle'>
			                    <Pair>
				                    <key>normal</key>
				                    <styleUrl>#licenseStyle-normal</styleUrl>
			                    </Pair>
			                    <Pair>
				                    <key>highlight</key>
				                    <styleUrl>#licenseStyle-highlight</styleUrl>
			                    </Pair>
		                    </StyleMap>");

            XElement ssn = XElement.Parse(@"<Style id='suburbStyle-normal'>
			                <LineStyle>
				                <color>ff3644DB</color>
				                <width>2</width>
			                </LineStyle>
			                <PolyStyle>
				                <color>7C3644DB</color>
				                <fill>1</fill>
				                <outline>1</outline>
			                </PolyStyle>
			                <BalloonStyle>
				                <text><![CDATA[<h3>$[name]</h3>]]></text>
			                </BalloonStyle>
		                </Style>");

            XElement ssh = XElement.Parse(@"<Style id='suburbStyle-highlight'>
			                <LineStyle>
				                <color>ff3644DB</color>
				                <width>3.0</width>
			                </LineStyle>
			                <PolyStyle>
				                <color>7C3644DB</color>
				                <fill>1</fill>
				                <outline>1</outline>
			                </PolyStyle>
			                <BalloonStyle>
				                <text><![CDATA[<h3>$[name]</h3>]]></text>
			                </BalloonStyle>
		                </Style>");

            XElement ss = XElement.Parse(@"<StyleMap id='suburbStyle'>
			                <Pair>
				                <key>normal</key>
				                <styleUrl>#suburbStyle-normal</styleUrl>
			                </Pair>
			                <Pair>
				                <key>highlight</key>
				                <styleUrl>#suburbStyle-highlight</styleUrl>
			                </Pair>
		                </StyleMap>");

            return new List<XElement>(new XElement[] { lsn, lsh, ls, ssn, ssh, ss });
        }

    }
}