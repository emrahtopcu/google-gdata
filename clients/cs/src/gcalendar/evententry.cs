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
using System.IO; 
using System.Collections;
using Google.GData.Client;
using Google.GData.Extensions;

namespace Google.GData.Calendar {

    //////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Entry API customization class for defining entries in an Event feed.
    /// </summary>
    //////////////////////////////////////////////////////////////////////
    public class EventEntry : AtomEntry
    {


        /// <summary>
        /// Category used to label entries that contain Event extension data.
        /// </summary>
        public static AtomCategory EVENT_CATEGORY =
        new AtomCategory(GDataParserNameTable.Event, new AtomUri(BaseNameTable.gKind));

        /// <summary>
        /// Constructs a new EventEntry instance with the appropriate category
        /// to indicate that it is an event.
        /// </summary>
        public EventEntry()
        : base()
        {
            Categories.Add(EVENT_CATEGORY);
            times = new WhenCollection(this);
            locations = new WhereCollection(this);
            participants = new WhoCollection(this);
        }


        /// <summary>
        /// Constructs a new EventStatus  instance
        /// </summary>
        public class EventStatus : EnumConstruct
        {
            /// <summary>
            ///  default constructor
            /// </summary>
            public EventStatus()
            : base(GDataParserNameTable.XmlEventStatusElement)
            {
            }

            /// <summary>
            ///  EventStatus constructor 
            /// </summary>
            /// <param name="value">indicates the default status</param>
            public EventStatus(string value)
            : base(GDataParserNameTable.XmlEventStatusElement, value)
            {
            }
            /// <summary>string constant for a confirmed event</summary>
            public const string CONFIRMED_VALUE = BaseNameTable.gNamespacePrefix + "event.confirmed";
            /// <summary>string constant for a tentative accepted event</summary>
            public const string TENTATIVE_VALUE = BaseNameTable.gNamespacePrefix + "event.tentative";
            /// <summary>string constant for a cancelled event</summary>
            public const string CANCELED_VALUE = BaseNameTable.gNamespacePrefix + "event.canceled";

            /// <summary>EventStatus constant for a confirmed event</summary>
            public static EventStatus CONFIRMED = new EventStatus(CONFIRMED_VALUE);
            /// <summary>EventStatus constant for a tentative event</summary>
            public static EventStatus TENTATIVE = new EventStatus(TENTATIVE_VALUE);
            /// <summary>EventStatus constant for a Cancelled  event</summary>
            public static EventStatus CANCELED = new EventStatus(CANCELED_VALUE);


            /// <summary>
            ///  parse method is called from the atom parser to populate an EventStatus node
            /// </summary>
            /// <param name="node">the xmlnode to parser</param>
            /// <returns>EventStatus object</returns>
            public static EventStatus parse(XmlNode node)
            {
                EventStatus eventStatus = null;
                if (String.Compare(node.NamespaceURI, BaseNameTable.gNamespace, true) == 0)
                {
                    eventStatus = new EventStatus();
                    if (node.Attributes != null)
                    {
                        eventStatus.Value = node.Attributes["value"].Value;
                    }
                }
                return eventStatus;
            }
        }

        /// <summary>
        /// Visibility class indicates the visibility of an eventNode
        /// </summary>
        public class Visibility : EnumConstruct
        {
            /// <summary>
            ///  default constructor
            /// </summary>
            public Visibility()
            : base(GDataParserNameTable.XmlVisibilityElement)
            {
            }

            /// <summary>
            ///  Visibility constructor with a string to indicate default value
            /// </summary>
            /// <param name="value">the default visibility value</param>
            public Visibility(string value)
            : base(GDataParserNameTable.XmlVisibilityElement, value)
            {
            }

            /// <summary>string constant for the default visibility value</summary>
            public const string DEFAULT_VALUE = BaseNameTable.gNamespacePrefix + "event.default";
            /// <summary>string constant for the public visibility value</summary>
            public const string PUBLIC_VALUE = BaseNameTable.gNamespacePrefix + "event.public";
            /// <summary>string constant for the confidential visibility value</summary>
            public const string CONFIDENTIAL_VALUE = BaseNameTable.gNamespacePrefix + "event.confidential";
            /// <summary>string constant for the private visibility value</summary>
            public const string PRIVATE_VALUE = BaseNameTable.gNamespacePrefix + "event.private";

            /// <summary>object constant for the private visibility value</summary>
            public static Visibility DEFAULT = new Visibility(DEFAULT_VALUE);
            /// <summary>object constant for the private visibility value</summary>
            public static Visibility PUBLIC = new Visibility(PUBLIC_VALUE);
            /// <summary>object constant for the private visibility value</summary>
            public static Visibility CONFIDENTIAL = new Visibility(CONFIDENTIAL_VALUE);
            /// <summary>object constant for the private visibility value</summary>
            public static Visibility PRIVATE = new Visibility(PRIVATE_VALUE);

            /// <summary>
            ///  parse method is called from the atom parser to populate an Visibility node
            /// </summary>
            /// <param name="node">the xmlnode to parser</param>
            /// <returns>Visibility object</returns>
            public static Visibility parse(XmlNode node)
            {
                Visibility vis = null;
                if (String.Compare(node.NamespaceURI, BaseNameTable.gNamespace, true) == 0)
                {
                    vis = new Visibility();
                    if (node.Attributes != null)
                    {
                        vis.Value = node.Attributes["value"].Value;
                    }
                }
                return vis;
            }
        }

        /// <summary>
        ///  the Transparency of an event class
        /// </summary>
        public class Transparency : EnumConstruct
        {
            /// <summary>
            ///  default constructor
            /// </summary>
            public Transparency()
            : base(GDataParserNameTable.XmlTransparencyElement)
            {
            }

            /// <summary>
            ///  constructor with a default string value
            /// </summary>
            /// <param name="value">transparency value</param>
            public Transparency(string value)
            : base(GDataParserNameTable.XmlTransparencyElement, value)
            {
            }

            /// <summary>string constant for the opaque transparency value</summary>
            public const string OPAQUE_VALUE = BaseNameTable.gNamespacePrefix + "event.opaque";
            /// <summary>string constant for the transparent trancparency value</summary>
            public const string TRANSPARENT_VALUE = BaseNameTable.gNamespacePrefix + "event.transparent";

            /// <summary>object constant for the opaque transparency value</summary>
            public static Transparency OPAQUE = new Transparency(OPAQUE_VALUE);
            /// <summary>object constant for the transparent transparency value</summary>
            public static Transparency TRANSPARENT = new Transparency(TRANSPARENT_VALUE);

            /// <summary>
            ///  parse method is called from the atom parser to populate an Transparency node
            /// </summary>
            /// <param name="node">the xmlnode to parser</param>
            /// <returns>Transparency object</returns>
            public static Transparency parse(XmlNode node)
            {
                Transparency trans = null;
                if (String.Compare(node.NamespaceURI, BaseNameTable.gNamespace, true) == 0)
                {
                    trans = new Transparency();
                    if (node.Attributes != null)
                    {
                        trans.Value = node.Attributes["value"].Value;
                    }
                }
                return trans;
            }
        }

#region EventEntry Attributes

        private WhenCollection times;
        private WhereCollection locations;
        private WhoCollection participants;
        private EventStatus status;
        private Visibility visibility;
        private Transparency transparency;
        private Recurrence recurrence;
        private OriginalEvent originalEvent;
        private Reminder reminder;
        private Comments comments;
        private RecurrenceException exception; 

#endregion

#region Public Methods
        /// <summary>
        ///  property accessor for the WhenCollection
        /// </summary>
        public WhenCollection Times
        {
            get { return times;}
        }

        /// <summary>
        ///  property accessor for the WhereCollection
        /// </summary>
        public WhereCollection Locations
        {
            get { return locations;}
        }

        /// <summary>
        ///  property accessor for the WhoCollection
        /// </summary>
        public WhoCollection Participants
        {
            get { return participants;}
        }

        /// <summary>
        ///  property accessor for the EventStatus
        /// </summary>
        public EventStatus Status
        {
            get { return status;}
            set
            {
                if (status != null)
                {
                    ExtensionElements.Remove(status);
                }
                status = value; 
                ExtensionElements.Add(status);
            }
        }

        /// <summary>
        ///  property accessor for the Event Visibility 
        /// </summary>
        public Visibility EventVisibility
        {
            get { return visibility;}
            set 
            {
                if (visibility != null)
                {
                    ExtensionElements.Remove(visibility);
                }
                visibility = value; 
                ExtensionElements.Add(visibility); 
            }
        }

        /// <summary>
        ///  property accessor for the EventTransparency
        /// </summary>
        public Transparency EventTransparency
        {
            get { return transparency;}
            set
            {
                if (transparency != null)
                {
                    ExtensionElements.Remove(transparency);
                }
                transparency = value; 
                ExtensionElements.Add(transparency);
            }
        }

        /// <summary>
        ///  property accessor for the Recurrence
        /// </summary>
        public Recurrence Recurrence
        {
            get { return recurrence;}
            set
            {
                if (recurrence != null)
                {
                    ExtensionElements.Remove(recurrence);
                }
                recurrence = value; 
                ExtensionElements.Add(recurrence);
            }
        }

      /// <summary>
      ///  property accessor for the RecurrenceException
      /// </summary>
      public RecurrenceException RecurrenceException
      {
          get { return exception;}
          set
          {
              if (exception != null)
              {
                  ExtensionElements.Remove(exception);
              }
              exception = value; 
              ExtensionElements.Add(exception);
          }
      }

        /// <summary>
        /// property accessor for the OriginalEvent
        /// </summary>
        public OriginalEvent OriginalEvent
        {
            get { return originalEvent;}
            set
            {
                if (originalEvent != null)
                {
                    ExtensionElements.Remove(originalEvent);
                }
                originalEvent = value; 
                ExtensionElements.Add(originalEvent);
            }
        }

        /// <summary>
        /// property accessor for the Reminder
        /// </summary>
        public Reminder Reminder
        {
            get 
            { 
                // if we are a recurrent event, reminder is on the entry/toplevel
                if (this.Recurrence != null)
                {
                    return reminder; 
                }
                else
                {
                    // in the non recurrent case, it's on the first when element
                    When w = this.Times != null ? this.Times[0] : null;
                    if (w != null)
                    {
                        return w.Reminder; 
                    }
                }
                return null; 
            }

            set
            {
                if (this.Recurrence != null)
                {
                    if (reminder != null)
                    {
                        ExtensionElements.Remove(reminder);
                    }
                    reminder = value; 
                    ExtensionElements.Add(reminder);
                }
                else
                {
                    // non recurring case, set it on the first when
                    // in the non recurrent case, it's on the first when element
                    When w = this.Times != null ? this.Times[0] : null;
                    if (w != null)
                    {
                        w.Reminder = value; 
                    }
                    else
                    {
                        throw new ArgumentException("Neither recurrence, nor a when object found. Please construct a when object, or the recurrence object first before setting a reminder time"); 
                    }
                }
            }
        }


        /// <summary>
        ///  property accessor for the Comments
        /// </summary>
        public Comments Comments
        {
            get { return comments;}
            set
            {
                if (comments != null)
                {
                    ExtensionElements.Remove(comments);
                }
                comments = value;
                ExtensionElements.Add(comments);
            }
        }

#endregion

        //////////////////////////////////////////////////////////////////////
        /// <summary>empty base implementation</summary> 
        /// <param name="writer">the xmlwriter, where we want to add default namespaces to</param>
        //////////////////////////////////////////////////////////////////////
        protected override void AddOtherNamespaces(XmlWriter writer) 
        {
            base.AddOtherNamespaces(writer); 
            Utilities.EnsureGDataNamespace(writer); 
        }
        /////////////////////////////////////////////////////////////////////////////

        //////////////////////////////////////////////////////////////////////
        /// <summary>checks if this is a namespace 
        /// decl that we already added</summary> 
        /// <param name="node">XmlNode to check</param>
        /// <returns>true if this node should be skipped </returns>
        //////////////////////////////////////////////////////////////////////
        protected override bool SkipNode(XmlNode node)
        {
            if (base.SkipNode(node)==true)
            {
                return true; 
            }

            Tracing.TraceMsg("in skipnode for node: " + node.Name + "--" + node.Value); 

            if (node.NodeType == XmlNodeType.Attribute && 
                (node.Name.StartsWith("xmlns") == true) && 
                (String.Compare(node.Value,BaseNameTable.gNamespace)==0))
                return true;
            return false; 
        }

#region Event Parser

        //////////////////////////////////////////////////////////////////////
        /// <summary>parses the inner state of the element</summary>
        /// <param name="eventNode">a g-scheme, xml node</param>
        /// <param name="parser">the atomFeedParser that called this</param>
        //////////////////////////////////////////////////////////////////////
        public void parseEvent(XmlNode eventNode, AtomFeedParser parser)
        {
            if (String.Compare(eventNode.NamespaceURI, BaseNameTable.gNamespace, true) == 0)
            {

                // Parse a When Element
                if (eventNode.LocalName == GDataParserNameTable.XmlWhenElement)
                {
                    this.Times.Add(When.ParseWhen(eventNode));
                }
                // Parse a Where Element
                else if (eventNode.LocalName == GDataParserNameTable.XmlWhereElement)
                {
                    this.Locations.Add((Where.ParseWhere(eventNode, parser)));
                }
                // Parse a Who Element
                else if (eventNode.LocalName == GDataParserNameTable.XmlWhoElement)
                {
                    this.Participants.Add((Who.ParseWho(eventNode, parser)));
                }
                // Parse a Status Element
                else if (eventNode.LocalName == GDataParserNameTable.XmlEventStatusElement)
                {
                    this.Status = EventStatus.parse(eventNode);
                }
                // Parse a Visibility Element
                else if (eventNode.LocalName == GDataParserNameTable.XmlVisibilityElement)
                {
                    this.EventVisibility = Visibility.parse(eventNode);
                }
                // Parse a Transparency Element
                else if (eventNode.LocalName == GDataParserNameTable.XmlTransparencyElement)
                {
                    this.EventTransparency = Transparency.parse(eventNode);
                }
                // Parse a Recurrence Element
                else if (eventNode.LocalName == GDataParserNameTable.XmlRecurrenceElement)
                {
                    this.Recurrence = Recurrence.ParseRecurrence(eventNode);
                }
                else if (eventNode.LocalName == GDataParserNameTable.XmlRecurrenceExceptionElement)
                {
                    this.RecurrenceException = RecurrenceException.ParseRecurrenceException(eventNode, parser);
                }
                // Parse an Original Event Element
                else if (eventNode.LocalName == GDataParserNameTable.XmlOriginalEventElement)
                {
                    this.OriginalEvent = OriginalEvent.ParseOriginal(eventNode);
                }
                // Parse a Reminder Element - recurrence event, g:reminder is in top level
                else if (eventNode.LocalName == GDataParserNameTable.XmlReminderElement)
                {
                    this.Reminder = Reminder.ParseReminder(eventNode);
                }
                // Parse a Comments Element
                else if (eventNode.LocalName == GDataParserNameTable.XmlCommentsElement)
                {
                    this.Comments = Comments.ParseComments(eventNode);
                }
            }
        }

#endregion

    }
}
