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
using System.IO;
using System.Collections;
using System.Text;
using System.Net; 
using Google.GData.Client;
using Google.GData.Extensions;
using System.Collections.Specialized;


namespace Google.GData.YouTube {

    //////////////////////////////////////////////////////////////////////
    /// <summary>
    /// The YouTube Data API allows applications to perform functions normally 
    /// executed on the YouTube website. The API enables your application to search 
    /// for YouTube videos and to retrieve standard video feeds, comments and video
    /// responses. 
    /// In addition, the API lets your application upload videos to YouTube or 
    /// update existing videos. Your can also retrieve playlists, subscriptions, 
    /// user profiles and more. Finally, your application can submit 
    /// authenticated requests to enable users to create playlists, 
    /// subscriptions, contacts and other account-specific entities.
    /// </summary>
    //////////////////////////////////////////////////////////////////////
    public class YouTubeService : MediaService
    {
       
        /// <summary>This service's User-Agent string</summary> 
        public const string YTAgent = "GYouTube-CS/1.0.0";
        /// <summary>The Calendar service's name</summary> 
        public const string YTService = "youtube";

        /// <summary>
        /// default category for YouTube
        /// </summary>
        public const string DefaultCategory = "http://gdata.youtube.com/schemas/2007/categories.cat"; 

        /// <summary>
        /// the YouTube authentication handler URL
        /// </summary>
        public const string AuthenticationHandler = "https://www.google.com/youtube/accounts/ClientLogin";


        private string clientID;
        private string developerID;


        /// <summary>
        ///  default constructor
        /// </summary>
        /// <param name="applicationName">the applicationname</param>
        /// <param name="client">the client identifier</param>
        /// <param name="developerKey">the developerKey</param>/// 
        public YouTubeService(string applicationName, string client, string developerKey) : base(YTService, applicationName, YTAgent)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client"); 
            }
            if (developerKey == null)
            {
                throw new ArgumentNullException("developerKey"); 
            }

            this.NewFeed += new ServiceEventHandler(this.OnNewFeed); 
            clientID = client;
            developerID = developerKey;
            this.ProtocolMajor = 2; 
            OnRequestFactoryChanged();
        }


        /// <summary>
        ///  readonly constructor 
        /// </summary>
        /// <param name="applicationName">the application identifier</param>
        public YouTubeService(string applicationName) : base(YTService, applicationName, YTAgent)
        {
            this.NewFeed += new ServiceEventHandler(this.OnNewFeed); 
            this.ProtocolMajor = 2; 
            OnRequestFactoryChanged();
        }
        /// <summary>
        /// overloaded to create typed version of Query
        /// </summary>
        /// <param name="feedQuery"></param>
        /// <returns>EventFeed</returns>
        public YouTubeFeed Query(YouTubeQuery feedQuery) 
        {
            return base.Query(feedQuery) as YouTubeFeed;
        }

        /// <summary>
        /// returns a playlist feed based on a youtubeQuery
        /// </summary>
        /// <param name="feedQuery"></param>
        /// <returns>EventFeed</returns>
        public PlaylistFeed GetPlaylist(YouTubeQuery feedQuery) 
        {
            return base.Query(feedQuery) as PlaylistFeed;
        }

        /// <summary>
        /// returns a playlist feed based on a youtubeQuery
        /// </summary>
        /// <param name="feedQuery"></param>
        /// <returns>EventFeed</returns>
        public FriendsFeed GetFriends(YouTubeQuery feedQuery) 
        {
            return base.Query(feedQuery) as FriendsFeed;
        }

        /// <summary>
        /// returns a playlists feed based on a youtubeQuery
        /// </summary>
        /// <param name="feedQuery"></param>
        /// <returns>EventFeed</returns>
        public PlaylistsFeed GetPlaylists(YouTubeQuery feedQuery) 
        {
            return base.Query(feedQuery) as PlaylistsFeed;
        }

        /// <summary>
        /// returns a subscription feed based on a youtubeQuery
        /// </summary>
        /// <param name="feedQuery"></param>
        /// <returns>EventFeed</returns>
        public SubscriptionFeed GetSubscriptions(YouTubeQuery feedQuery) 
        {
            return base.Query(feedQuery) as SubscriptionFeed;
        }

        /// <summary>
        /// returns a message feed based on a youtubeQuery
        /// </summary>
        /// <param name="feedQuery"></param>
        /// <returns>EventFeed</returns>
        public MessageFeed GetMessages(YouTubeQuery feedQuery) 
        {
            return base.Query(feedQuery) as MessageFeed;
        }
        /// <summary>
        /// upload a new video to this users youtube account
        /// </summary>
        /// <param name="userName">the username (account) to use</param>
        /// <param name="entry">the youtube entry</param>
        /// <returns></returns>
        public YouTubeEntry Upload(string userName, YouTubeEntry entry)
        {
            Uri uri = new Uri("http://uploads.gdata.youtube.com/feeds/api/users/" + userName + "/uploads");
            return base.Insert(uri, entry); 
        }

        /// <summary>
        /// upload a new video to the default/authenticated account
        /// </summary>
        /// <param name="entry">the youtube entry</param>
        /// <returns></returns>
        public YouTubeEntry Upload(YouTubeEntry entry)
        {
            Uri uri = new Uri("http://uploads.gdata.youtube.com/feeds/api/users/default/uploads");
            return base.Insert(uri, entry); 
        }

        
        /// <summary>
        /// Method for browser-based upload, gets back a non-Atom response
        /// </summary>
        /// <param name="newEntry">The YouTubeEntry containing the metadata for a video upload</param>
        /// <returns>A FormUploadToken object containing an upload token and POST url</returns>
        public FormUploadToken FormUpload(YouTubeEntry newEntry)
        {
            Uri uri = new Uri("http://gdata.youtube.com/action/GetUploadToken");
            
            if (newEntry == null)
            {
                throw new ArgumentNullException("newEntry");
            }

            Stream returnStream = EntrySend(uri, newEntry, GDataRequestType.Insert);

            FormUploadToken token = new FormUploadToken(returnStream);

            returnStream.Close();

            return token;
        }

      
        /// <summary>
        /// notifier if someone changes the requestfactory of the service
        /// </summary>
        public override void OnRequestFactoryChanged() 
        {
            base.OnRequestFactoryChanged();
            GDataGAuthRequestFactory factory = this.RequestFactory as GDataGAuthRequestFactory;
            if (factory != null && this.developerID != null && this.clientID != null)
            {
                RemoveOldKeys(factory.CustomHeaders);
                factory.CustomHeaders.Add(GoogleAuthentication.YouTubeClientId + this.clientID); 
                factory.CustomHeaders.Add(GoogleAuthentication.YouTubeDevKey + this.developerID); 
                factory.Handler = YouTubeService.AuthenticationHandler;
            }
        }

        private static void RemoveOldKeys(StringCollection headers)
        {
            foreach (string header in headers)
            {
                if (header.StartsWith(GoogleAuthentication.WebKey))
                {
                    headers.Remove(header);
                    return;
                }
            }
            return;
        }



        //////////////////////////////////////////////////////////////////////
        /// <summary>eventchaining. We catch this by from the base service, which 
        /// would not by default create an atomFeed</summary> 
        /// <param name="sender"> the object which send the event</param>
        /// <param name="e">FeedParserEventArguments, holds the feedentry</param> 
        /// <returns> </returns>
        //////////////////////////////////////////////////////////////////////
        protected void OnNewFeed(object sender, ServiceEventArgs e)
        {
            Tracing.TraceMsg("Created new YouTube Feed");
            if (e == null)
            {
                throw new ArgumentNullException("e"); 
            }

            if (e.Uri.AbsolutePath.IndexOf("feeds/api/playlists/") != -1)
            {
                // playlists base url: http://gdata.youtube.com/feeds/api/playlists/
                e.Feed = new PlaylistFeed(e.Uri, e.Service);
            } 
            else if(e.Uri.AbsolutePath.IndexOf("feeds/api") != -1 && 
                    e.Uri.AbsolutePath.IndexOf("contacts") != -1)
            {
                // contacts feeds are http://gdata.youtube.com/feeds/api/users/username/contacts
                e.Feed = new FriendsFeed(e.Uri, e.Service);
            }
            else if(e.Uri.AbsolutePath.IndexOf("feeds/api/users") != -1 && 
                    e.Uri.AbsolutePath.IndexOf("playlists") != -1)
            {
                // user based list of playlists are http://gdata.youtube.com/feeds/api/users/username/playlists
                e.Feed = new PlaylistsFeed(e.Uri, e.Service);
            }
            else if(e.Uri.AbsolutePath.IndexOf("feeds/api/users") != -1 && 
                    e.Uri.AbsolutePath.IndexOf("subscriptions") != -1)
            {
                // user based list of subscriptions are http://gdata.youtube.com/feeds/api/users/username/subscriptions
                e.Feed = new SubscriptionFeed(e.Uri, e.Service);
            }
            else if(e.Uri.AbsolutePath.IndexOf("feeds/api/users") != -1 && 
                    e.Uri.AbsolutePath.IndexOf("inbox") != -1)
            {
                // user based list of messages are http://gdata.youtube.com/feeds/api/users/username/inbox
                e.Feed = new MessageFeed(e.Uri, e.Service);
            }
            else if(e.Uri.AbsolutePath.IndexOf("feeds/api/videos") != -1 && 
                    e.Uri.AbsolutePath.IndexOf("comments") != -1)
            {
                // user based list of messages are http://gdata.youtube.com/feeds/api/videos/videoid/comments
                e.Feed = new CommentsFeed(e.Uri, e.Service);
            }
            else if (e.Uri.AbsolutePath.IndexOf("feeds/api/users") != -1 &&
               e.Uri.AbsolutePath.IndexOf("uploads") != -1)
            {
                // user based upload service are http://gdata.youtube.com/feeds/api/users/videoid/uploads
                e.Feed = new YouTubeFeed(e.Uri, e.Service);
            }

            else if(IsProfileUri(e.Uri))
            {
                // user based list of playlists are http://gdata.youtube.com/feeds/api/users/username/playlists
                e.Feed = new ProfileFeed(e.Uri, e.Service);
            }
            else 
            {
                // everything not detected yet, is a youtubefeed.
                e.Feed = new YouTubeFeed(e.Uri, e.Service);
            }
        }
        /////////////////////////////////////////////////////////////////////////////

        private bool IsProfileUri(Uri uri)
        {
            String str = uri.AbsolutePath;
            if (str.StartsWith("/") == true)
            {
                str = str.Substring(1);
            }
            if (str.EndsWith("/") == true)
            {
                // remove the last char
                str.Remove(str.Length - 1, 1);
            }
            if (str.StartsWith("feeds/api/users/") == true)
            {
                str = str.Substring(16);
                // now there should be one word left, no more slashes
                if (str.IndexOf('/') == -1)
                    return true;
            }
            return false;
        }
    }
}
