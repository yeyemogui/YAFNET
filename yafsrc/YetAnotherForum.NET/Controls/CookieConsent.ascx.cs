/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2021 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Controls
{
    #region Using

    using System;
    using System.Web;

    using YAF.Core.BaseControls;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;

    using DateTime = System.DateTime;

    #endregion

    /// <summary>
    /// The forum control for showing the cookie warning!
    /// </summary>
    public partial class CookieConsent : BaseUserControl
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.Label1.Param0 = this.PageContext.BoardSettings.Name;

            this.MoreDetails.NavigateUrl = this.Get<LinkBuilder>().GetLink(ForumPages.Cookies);
        }

        /// <summary>
        /// Accept Cookie Consent
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void AcceptClick(object sender, EventArgs e)
        {
            this.PageContext.Get<HttpResponseBase>().SetCookie(
                new HttpCookie("YAF-AcceptCookies", "true")
                {
                    Expires = DateTime.UtcNow.AddYears(1), HttpOnly = false
                });

            this.Response.Redirect(this.Request.RawUrl);

            this.DataBind();
        }
    }
}