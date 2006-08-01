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
#region Using directives

#define USE_TRACING

using System;
using System.Collections;

#endregion

//////////////////////////////////////////////////////////////////////
// contains typed collections based on the 1.1 .NET framework
// using typed collections has the benefit of additional code reliability
// and using them in the collection editor
// 
//////////////////////////////////////////////////////////////////////
namespace Google.GData.Client
{

    //////////////////////////////////////////////////////////////////////
    /// <summary>standard typed collection based on 1.1 framework for FeedEntries
    /// </summary> 
    //////////////////////////////////////////////////////////////////////
    public class AtomEntryCollection : CollectionBase  
    {
        /// <summary>holds the owning feed</summary> 
        private AtomFeed feed;

        /// <summary>private default constructor</summary> 
        private AtomEntryCollection()
        {
        }
        /// <summary>constructor</summary> 
        public AtomEntryCollection(AtomFeed feed) : base()
        {
            this.feed = feed; 
        }

        /// <summary>standard typed accessor method </summary> 
        public AtomEntry this[ int index ]  
        {
            get  
            {
                return( (AtomEntry) List[index] );
            }
            set  
            {
                if (value.Feed == null || value.Feed != this.feed)
                {
                    value.setFeed(this.feed);
                }
                List[index] = value;
            }
        }

        /// <summary>standard typed add method </summary> 
        public int Add( AtomEntry value )  
        {
            if (value != null)
            {
                if (value.Feed == null)
                {
                    value.setFeed(this.feed);
                }
                else
                {
                    if (this.feed != null && value.Feed == this.feed)
                    {
                        // same object, already in here. 
                        throw new ArgumentException("The entry is already part of this collection");
                    }
                    // now we need to see if this is the same feed. If not, copy
                    if (AtomFeed.IsFeedIdentical(value.Feed, this.feed) == false)
                    {
                        AtomEntry newEntry = AtomEntry.ImportFromFeed(value); 
                        newEntry.setFeed(this.feed);
                        value = newEntry; 
                    }
                }
            }
            return( List.Add( value ) );
        }

        /// <summary>standard typed indexOf method </summary> 
        public int IndexOf( AtomEntry value )  
        {
            return( List.IndexOf( value ) );
        }
    
        /// <summary>standard typed insert method </summary> 
        public void Insert( int index, AtomEntry value )  
        {
            List.Insert( index, value );
        }

        /// <summary>standard typed remove method </summary> 
        public void Remove( AtomEntry value )  
        {
            List.Remove( value );
        }

        /// <summary>standard typed Contains method </summary> 
        public bool Contains( AtomEntry value )  
        {
            // If value is not of type AtomEntry, this will return false.
            return( List.Contains( value ) );
        }

        /// <summary>standard typed OnValidate Override </summary> 
        protected override void OnValidate( Object value )  
        {
            if ( value as AtomEntry == null)
                throw new ArgumentException( "value must be of type Google.GData.Client.AtomEntry.", "value" );
        }
    }
    /////////////////////////////////////////////////////////////////////////////

    //////////////////////////////////////////////////////////////////////
    /// <summary>standard typed collection based on 1.1 framework for AtomLinks
    /// </summary> 
    //////////////////////////////////////////////////////////////////////
    public class AtomLinkCollection : CollectionBase  
    {
        /// <summary>standard typed accessor method </summary> 
        public AtomLink this[ int index ]  
        {
            get  
            {
                return( (AtomLink) List[index] );
            }
            set  
            {
                List[index] = value;
            }
        }

        /// <summary>standard typed accessor method </summary> 
        public int Add( AtomLink value )  
        {
	    // Remove link with same relation to avoid duplication.
	    AtomLink oldLink = FindService(value.Rel, value.Type);
	    if (oldLink != null)
	    {
	        List.Remove(oldLink);
	    }
            return( List.Add( value ) );
        }
        /// <summary>standard typed accessor method </summary> 
        public int IndexOf( AtomLink value )  
        {
            return( List.IndexOf( value ) );
        }
        /// <summary>standard typed accessor method </summary> 
        public void Insert( int index, AtomLink value )  
        {
            List.Insert( index, value );
        }
        /// <summary>standard typed accessor method </summary> 
        public void Remove( AtomLink value )  
        {
            List.Remove( value );
        }
        /// <summary>standard typed accessor method </summary> 
        public bool Contains( AtomLink value )  
        {
            // If value is not of type AtomLink, this will return false.
            return( List.Contains( value ) );
        }

        //////////////////////////////////////////////////////////////////////
        /// <summary>public AtomLink FindService(string service,string type)
        ///   Retrieves the first link with the supplied 'rel' and/or 'type' value.
        ///   If either parameter is null, the corresponding match isn't needed.
        /// </summary> 
        /// <param name="service">the service entry to find</param>
        /// <param name="type">the link type to find</param>
        /// <returns>the found link or NULL </returns>
        //////////////////////////////////////////////////////////////////////
        public AtomLink FindService(string service, string type)
        {
            foreach (AtomLink link in List )
            {
                string linkRel = link.Rel;
                string linkType = link.Type;

                if ((service == null || (linkRel != null && linkRel == service )) &&
                    (type == null || (linkType != null && linkType == type))) {

                  return link;
                }
            }
            Tracing.TraceMsg("Found NULL service, probably mistake");
            return null;
        }
        /////////////////////////////////////////////////////////////////////////////


        /// <summary>standard typed accessor method </summary> 
        protected override void OnValidate( Object value )  
        {
            if ( value.GetType() != Type.GetType("Google.GData.Client.AtomLink") )
                throw new ArgumentException( "value must be of type Google.GData.Client.AtomLink.", "value" );
        }
    }
    /////////////////////////////////////////////////////////////////////////////

    //////////////////////////////////////////////////////////////////////
    /// <summary>standard typed collection based on 1.1 framework for AtomCategory
    /// </summary> 
    //////////////////////////////////////////////////////////////////////
    public class AtomCategoryCollection : CollectionBase  
    {
        /// <summary>standard typed accessor method </summary> 
        public AtomCategory this[ int index ]  
        {
            get  
            {
                return( (AtomCategory) List[index] );
            }
            set  
            {
                List[index] = value;
            }
        }
        /// <summary>standard typed accessor method </summary> 
        public int Add( AtomCategory value )  
        {
	    // Remove category with the same term to avoid duplication.
	    AtomCategory oldCategory = FindCategory(value.Term);
	    if (oldCategory != null)
	    {
	        List.Remove(oldCategory);
	    }
            return( List.Add( value ) );
        }
        /// <summary>standard typed accessor method </summary> 
        public int IndexOf( AtomCategory value )  
        {
            return( List.IndexOf( value ) );
        }
        /// <summary>standard typed accessor method </summary> 
        public void Insert( int index, AtomCategory value )  
        {
            List.Insert( index, value );
        }
        /// <summary>standard typed accessor method </summary> 
        public void Remove( AtomCategory value )  
        {
            List.Remove( value );
        }
        /// <summary>standard typed accessor method </summary> 
        public bool Contains( AtomCategory value )  
        {
            // If value is not of type AtomCategory, this will return false.
            return( List.Contains( value ) );
        }
        /// <summary>standard typed accessor method </summary> 
        protected override void OnValidate( Object value )  
        {
            if ( value.GetType() != Type.GetType("Google.GData.Client.AtomCategory") )
                throw new ArgumentException( "value must be of type Google.GData.Client.AtomCategory.", "value" );
        }
	/// <summary>retrieves the first category with the matching value</summary>
	protected virtual AtomCategory FindCategory(string term)
	{
	    foreach (AtomCategory category in List)
	    {
	        if (term == category.Term)
		{
		    return category;
		}
	    }
	    return null;
	}
    }
    /////////////////////////////////////////////////////////////////////////////

    //////////////////////////////////////////////////////////////////////
    /// <summary>standard typed collection based on 1.1 framework for AtomPerson
    /// </summary> 
    //////////////////////////////////////////////////////////////////////
    public class QueryCategoryCollection : CollectionBase  
    {
        /// <summary>standard typed accessor method </summary> 
        public QueryCategory this[ int index ]  
        {
            get  
            {
                return( (QueryCategory) List[index] );
            }
            set  
            {
                List[index] = value;
            }
        }
        /// <summary>standard typed accessor method </summary> 
        public int Add( QueryCategory value )  
        {
            return( List.Add( value ) );
        }
        /// <summary>standard typed accessor method </summary> 
        public int IndexOf( QueryCategory value )  
        {
            return( List.IndexOf( value ) );
        }
        /// <summary>standard typed accessor method </summary> 
        public void Insert( int index, QueryCategory value )  
        {
            List.Insert( index, value );
        }
        /// <summary>standard typed accessor method </summary> 
        public void Remove( QueryCategory value )  
        {
            List.Remove( value );
        }
        /// <summary>standard typed accessor method </summary> 
        public bool Contains( QueryCategory value )  
        {
            // If value is not of type AtomPerson, this will return false.
            return( List.Contains( value ) );
        }
        /// <summary>standard typed accessor method </summary> 
        protected override void OnValidate( Object value )  
        {
            if ( value.GetType() != Type.GetType("Google.GData.Client.QueryCategory") )
                throw new ArgumentException( "value must be of type Google.GData.Client.QueryCategory.", "value" );
        }
    }
    /////////////////////////////////////////////////////////////////////////////


    //////////////////////////////////////////////////////////////////////
    /// <summary>standard typed collection based on 1.1 framework for AtomPerson
    /// </summary> 
    //////////////////////////////////////////////////////////////////////
    public class AtomPersonCollection : CollectionBase  
    {
        /// <summary>standard typed accessor method </summary> 
        public AtomPerson this[ int index ]  
        {
            get  
            {
                return( (AtomPerson) List[index] );
            }
            set  
            {
                List[index] = value;
            }
        }
        /// <summary>standard typed accessor method </summary> 
        public int Add( AtomPerson value )  
        {
            return( List.Add( value ) );
        }
        /// <summary>standard typed accessor method </summary> 
        public int IndexOf( AtomPerson value )  
        {
            return( List.IndexOf( value ) );
        }
        /// <summary>standard typed accessor method </summary> 
        public void Insert( int index, AtomPerson value )  
        {
            List.Insert( index, value );
        }
        /// <summary>standard typed accessor method </summary> 
        public void Remove( AtomPerson value )  
        {
            List.Remove( value );
        }
        /// <summary>standard typed accessor method </summary> 
        public bool Contains( AtomPerson value )  
        {
            // If value is not of type AtomPerson, this will return false.
            return( List.Contains( value ) );
        }
        /// <summary>standard typed accessor method </summary> 
        protected override void OnValidate( Object value )  
        {
            if ( value.GetType() != Type.GetType("Google.GData.Client.AtomPerson") )
                throw new ArgumentException( "value must be of type Google.GData.Client.AtomPerson.", "value" );
        }
    }
    /////////////////////////////////////////////////////////////////////////////

} 