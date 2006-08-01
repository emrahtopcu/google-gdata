/* Copyright (c) 2006 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

using System;
using System.Xml;
using System.Collections;
using System.Text;
using Google.GData.Client;

namespace Google.GData.Extensions 
{

    /// <summary>
    /// GData schema extension describing a period of time.
    /// </summary>
    public class When : IExtensionElement
    {

        /// <summary>
        /// Event start time (required).
        /// </summary>
        protected DateTime startTime;

        /// <summary>
        /// Event end time (optional).
        /// </summary>
        protected DateTime endTime;

        /// <summary>
        /// String description of the event times.
        /// </summary>
        protected String valueString;

        /// <summary>
        /// flag, indicating if an all day status
        /// </summary>
        protected bool fAllDay; 
        
        /// <summary>
        /// reminder object to set reminder durations
        /// </summary>
        protected Reminder reminder; 

        //////////////////////////////////////////////////////////////////////
        /// <summary>accessor method public DateTime StartTime</summary> 
        /// <returns> </returns>
        //////////////////////////////////////////////////////////////////////
        public DateTime StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }

        //////////////////////////////////////////////////////////////////////
        /// <summary>accessor method public DateTime EndTime</summary> 
        /// <returns> </returns>
        //////////////////////////////////////////////////////////////////////
        public DateTime EndTime
        {
            get { return endTime; }
            set { endTime = value; }
        }

        //////////////////////////////////////////////////////////////////////
        /// <summary>reminder accessor</summary> 
        //////////////////////////////////////////////////////////////////////
        public Reminder Reminder
        {
            get { return this.reminder; }
            set { this.reminder = value; }
        }
        //////////////////////////////////////////////////////////////////////
        /// <summary>accessor method public string ValueString</summary> 
        /// <returns> </returns>
        //////////////////////////////////////////////////////////////////////
        public String ValueString
        {
            get { return valueString; }
            set { valueString = value; }
        }


        //////////////////////////////////////////////////////////////////////
        /// <summary>accessor method to the allday event flag</summary>
        /// <returns>true if it's an all day event</returns>
        //////////////////////////////////////////////////////////////////////
        public bool AllDay
        {
            get { return this.fAllDay; }
            set { this.fAllDay = value; }
        }


        #region When Parser
        //////////////////////////////////////////////////////////////////////
        /// <summary>Parses an xml node to create an When object.</summary> 
        /// <param name="node">when node</param>
        /// <returns>the created When object</returns>
        //////////////////////////////////////////////////////////////////////
        public static When ParseWhen(XmlNode node)
        {
            Tracing.TraceCall();
            When when = null;
            Tracing.Assert(node != null, "node should not be null");
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            bool startTimeFlag = false, endTimeFlag = false;
            object localname = node.LocalName;
            if (localname.Equals(GDataParserNameTable.XmlWhenElement))
            {
                when = new When();
                if (node.Attributes != null)
                {
                    String value = node.Attributes[GDataParserNameTable.XmlAttributeStartTime] != null ? 
                        node.Attributes[GDataParserNameTable.XmlAttributeStartTime].Value : null; 
                    if (value != null)
                    {
                        startTimeFlag = true;
                        when.startTime = DateTime.Parse(value);
                        when.AllDay = (value.IndexOf('T') == -1); 
                    }

                    value = node.Attributes[GDataParserNameTable.XmlAttributeEndTime] != null ? 
                        node.Attributes[GDataParserNameTable.XmlAttributeEndTime].Value : null; 

                    if (value != null)
                    {
                        endTimeFlag = true;
                        when.endTime = DateTime.Parse(value); 
                        when.AllDay = when.AllDay && (value.IndexOf('T') == -1); 
                    }

                    if (node.Attributes[GDataParserNameTable.XmlAttributeValueString] != null)
                    {
                        when.valueString = node.Attributes[GDataParserNameTable.XmlAttributeValueString].Value;
                    }
                }
                // single event, g:reminder is inside g:when
                if (node.HasChildNodes)
                {
                    foreach (XmlNode whenChildNode in node.ChildNodes)
                    {
                        if (whenChildNode.LocalName == GDataParserNameTable.XmlReminderElement)
                        {
                            when.Reminder = Reminder.ParseReminder(whenChildNode);
                        }
                    }
                }
            }

            if (!startTimeFlag)
            {
                throw new ArgumentNullException("g:when/@startTime is required.");
            }

            if (endTimeFlag && when.startTime.CompareTo(when.endTime) > 0)
            {
                throw new ArgumentException("g:when/@startTime must be less than or equal to g:when/@endTime.");
            }

            return when;
        }
        #endregion

        #region overloaded for persistence

        //////////////////////////////////////////////////////////////////////
        /// <summary>Returns the constant representing this XML element.
        /// </summary> 
        //////////////////////////////////////////////////////////////////////
        public string XmlName
        {
            get { return GDataParserNameTable.XmlWhenElement; }
        }

        /// <summary>
        /// Persistence method for the When object
        /// </summary>
        /// <param name="writer">the xmlwriter to write into</param>
        public void Save(XmlWriter writer)
        {

            if (Utilities.IsPersistable(this.valueString) ||
                this.startTime != new DateTime(1,1,1) || 
                this.endTime != new DateTime(1,1,1))

            {
                writer.WriteStartElement(BaseNameTable.gDataPrefix, XmlName, BaseNameTable.gNamespace);
                if (startTime != new DateTime(1, 1, 1))
                {

                    string date = this.fAllDay ? Utilities.LocalDateInUTC(this.startTime) 
                                                : Utilities.LocalDateTimeInUTC(this.startTime);
                    writer.WriteAttributeString(GDataParserNameTable.XmlAttributeStartTime, date);
                }
                else
                {
                    throw new ArgumentNullException("g:when/@startTime is required.");
                }
    
                if (endTime != new DateTime(1, 1, 1))
                {
                    string date = this.fAllDay ? Utilities.LocalDateInUTC(this.endTime) 
                                                : Utilities.LocalDateTimeInUTC(this.endTime);
                    writer.WriteAttributeString(GDataParserNameTable.XmlAttributeEndTime, date);
                }
    
                if (Utilities.IsPersistable(this.valueString))
                {
                    writer.WriteAttributeString(GDataParserNameTable.XmlAttributeValueString, this.valueString);
                }
                if (this.Reminder != null)
                {
                    this.Reminder.Save(writer);
                }
                
    
                writer.WriteEndElement();
            }
        }
        #endregion
    }
}