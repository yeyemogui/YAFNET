/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2024 Ingo Herbote
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

namespace YAF.Pages.Admin;

using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc.Rendering;

using YAF.Configuration;
using YAF.Core.Extensions;
using YAF.Core.Services;
using YAF.Types.Extensions;
using YAF.Types.Extensions.Data;
using YAF.Types.Interfaces.Data;

/// <summary>
/// The Host Settings Page.
/// </summary>
public class HostSettingsModel : AdminPage
{
    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public HostSettingsInputModel Input { get; set; }

    [BindProperty]
    public List<SelectListItem> SpamServiceTypeList { get; set; }

    [BindProperty]
    public List<SelectListItem> BotSpamServiceTypeList { get; set; }

    [BindProperty]
    public List<SelectListItem> SpamMessageHandlingList { get; set; }

    [BindProperty]
    public List<SelectListItem> BotHandlingOnRegisterList { get; set; }

    [BindProperty]
    public List<SelectListItem> SendWelcomeNotificationAfterRegisterList { get; set; }

    [BindProperty]
    public List<SelectListItem> PostsFeedAccessList { get; set; }

    [BindProperty]
    public List<SelectListItem> AllowCreateTopicsSameNameList { get; set; }

    [BindProperty]
    public List<SelectListItem> PostLatestFeedAccessList { get; set; }

    [BindProperty]
    public List<SelectListItem> TopicsFeedAccessList { get; set; }

    [BindProperty]
    public List<SelectListItem> ReportPostPermissionsList { get; set; }

    [BindProperty]
    public List<SelectListItem> ProfileViewPermissionsList { get; set; }

    [BindProperty]
    public List<SelectListItem> MembersListViewPermissionsList { get; set; }

    [BindProperty]
    public List<SelectListItem> ActiveUsersViewPermissionsList { get; set; }

    [BindProperty]
    public List<SelectListItem> SearchPermissionsList { get; set; }

    [BindProperty]
    public List<SelectListItem> ShowHelpToList { get; set; }

    [BindProperty]
    public List<SelectListItem> ShowTeamToList { get; set; }

    [BindProperty]
    public List<SelectListItem> ShowShareTopicToList { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HostSettingsModel"/> class.
    /// </summary>
    public HostSettingsModel()
        : base("ADMIN_HOSTSETTINGS", ForumPages.Admin_HostSettings)
    {
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddAdminIndex();
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_HOSTSETTINGS", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Updates the Search Index
    /// </summary>
    public IActionResult OnPostIndexSearch()
    {
        this.PageBoardContext.BoardSettings.ForceUpdateSearchIndex = true;

        this.Get<BoardSettingsService>().SaveRegistry(this.PageBoardContext.BoardSettings);

        this.BindData();

        return this.PageBoardContext.Notify(this.GetText("FORCE_SEARCHINDED"), MessageTypes.info);
    }

    /// <summary>
    /// Resets the Active Discussions Cache
    /// </summary>
    public void OnPostActiveDiscussionsCacheReset()
    {
        this.RemoveCacheKey(Constants.Cache.ActiveDiscussions);

        this.BindData();
    }

    /// <summary>
    /// Resets the Board Categories Cache
    /// </summary>
    public void OnPostBoardModeratorsCacheReset()
    {
        this.RemoveCacheKey(Constants.Cache.ForumModerators);

        this.BindData();
    }

    /// <summary>
    /// Resets the Board User Stats Cache
    /// </summary>
    public void OnPostBoardUserStatsCacheReset()
    {
        this.RemoveCacheKey(Constants.Cache.BoardUserStats);

        this.BindData();
    }

    /// <summary>
    /// Resets the User Lazy Data Cache
    /// </summary>
    public void OnPostUserLazyDataCacheReset()
    {
        this.Get<IDataCache>().RemoveOf<object>(
            k => k.Key.StartsWith(string.Format(Constants.Cache.ActiveUserLazyData, string.Empty)));

        this.BindData();
    }

    /// <summary>
    /// Resets the Forum Statistics Cache
    /// </summary>
    public void OnPostForumStatisticsCacheReset()
    {
        this.RemoveCacheKey(Constants.Cache.BoardStats);

        this.BindData();
    }

    /// <summary>
    /// Resets the Complete Cache
    /// </summary>
    public void OnPostResetCacheAll()
    {
        // clear all cache keys
        this.Get<IObjectStore>().Clear();
        this.Get<IDataCache>().Clear();

        this.BindData();
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public IActionResult OnGet()
    {
        this.Input = new HostSettingsInputModel();

        if (!this.PageBoardContext.PageUser.UserFlags.IsHostAdmin)
        {
            return this.Get<LinkBuilder>().AccessDenied();
        }

        this.RenderListItems();

        this.BindData();

        return this.Page();
    }

    /// <summary>
    /// Saves the Host Settings
    /// </summary>
    public IActionResult OnPostSave()
    {
        // write all the settings back to the settings class

        // load Board Setting collection information...
        var settingCollection = new BoardSettingCollection(this.PageBoardContext.BoardSettings);

        // handle checked fields...
        settingCollection.SettingsBool.Keys.ForEach(
            name =>
            {
                var propertyInfo = this.Input.GetType().GetProperty(name);

                if (propertyInfo is null || propertyInfo.PropertyType.Name != "Boolean"
                                         || !settingCollection.SettingsBool[name].CanWrite)
                {
                    return;
                }

                var value = propertyInfo.GetValue(this.Input, null);

                settingCollection.SettingsBool[name].SetValue(this.PageBoardContext.BoardSettings, value, null);
            });

        // handle string fields...
        settingCollection.SettingsString.Keys.ForEach(
            name =>
            {
                var propertyInfo = this.Input.GetType().GetProperty(name);

                if (propertyInfo is null || propertyInfo.PropertyType.Name != "String"
                                         || !settingCollection.SettingsString[name].CanWrite)
                {
                    return;
                }

                var value = propertyInfo.GetValue(this.Input, null);

                settingCollection.SettingsString[name].SetValue(this.PageBoardContext.BoardSettings, value, null);
            });

        // handle int fields...
        settingCollection.SettingsInt.Keys.ForEach(
            name =>
            {
                var propertyInfo = this.Input.GetType().GetProperty(name);

                if (propertyInfo is null || !settingCollection.SettingsInt[name].CanWrite)
                {
                    return;
                }

                switch (propertyInfo.PropertyType.Name)
                {
                    case "Int32":
                    {
                        var value = propertyInfo.GetValue(this.Input, null);

                        settingCollection.SettingsInt[name].SetValue(this.PageBoardContext.BoardSettings, value, null);
                        break;
                    }
                    case "String":
                    {
                        var value = propertyInfo.GetValue(this.Input, null);

                        settingCollection.SettingsInt[name].SetValue(this.PageBoardContext.BoardSettings, value.ToType<int>(), null);
                        break;
                    }
                }
            });

        // save the settings to the database
        this.Get<BoardSettingsService>().SaveRegistry(this.PageBoardContext.BoardSettings);

        return this.Get<LinkBuilder>().Redirect(ForumPages.Admin_Admin);
    }

    /// <summary>
    /// Fill Lists with Localized Items
    /// </summary>
    private void RenderListItems()
    {
        var localizations = new[] {"FORBIDDEN", "REG_USERS", "ALL_USERS"};

        var dropDownLists = new List<SelectListItem>();

        dropDownLists.AddRange(
            localizations.Select((t, i) => new SelectListItem(this.GetText("ADMIN_HOSTSETTINGS", t), i.ToString()))
                .ToArray());

        this.PostsFeedAccessList = dropDownLists;
        this.AllowCreateTopicsSameNameList = dropDownLists;
        this.PostLatestFeedAccessList = dropDownLists;
        this.TopicsFeedAccessList = dropDownLists;
        this.ReportPostPermissionsList = dropDownLists;
        this.ProfileViewPermissionsList = dropDownLists;
        this.MembersListViewPermissionsList = dropDownLists;
        this.ActiveUsersViewPermissionsList = dropDownLists;
        this.SearchPermissionsList = dropDownLists;
        this.ShowHelpToList = dropDownLists;
        this.ShowTeamToList = dropDownLists;
        this.ShowShareTopicToList = dropDownLists;

        this.SpamServiceTypeList = [
            new(this.GetText("ADMIN_COMMON", "DISABLED"), "0"),
            new(
                this.GetText(
                    "ADMIN_HOSTSETTINGS",
                    "SPAM_SERVICE_TYP_3"),
                "3")
        ];

        this.BotSpamServiceTypeList = [
            new(this.GetText("ADMIN_COMMON", "DISABLED"), "0"),
            new(
                this.GetText(
                    "ADMIN_HOSTSETTINGS",
                    "BOT_CHECK_1"),
                "1"),

            new(
                this.GetText(
                    "ADMIN_HOSTSETTINGS",
                    "BOT_CHECK_2"),
                "2"),

            new(
                this.GetText(
                    "ADMIN_HOSTSETTINGS",
                    "BOT_CHECK_4"),
                "4")
        ];

        this.SpamMessageHandlingList = [
            new(
                this.GetText(
                    "ADMIN_HOSTSETTINGS",
                    "SPAM_MESSAGE_0"),
                "0"),

            new(
                this.GetText(
                    "ADMIN_HOSTSETTINGS",
                    "SPAM_MESSAGE_1"),
                "1"),

            new(
                this.GetText(
                    "ADMIN_HOSTSETTINGS",
                    "SPAM_MESSAGE_2"),
                "2"),

            new(
                this.GetText(
                    "ADMIN_HOSTSETTINGS",
                    "SPAM_MESSAGE_3"),
                "3")
        ];

        this.BotHandlingOnRegisterList = [
            new(
                this.GetText(
                    "ADMIN_HOSTSETTINGS",
                    "BOT_MESSAGE_0"),
                "0"),

            new(
                this.GetText(
                    "ADMIN_HOSTSETTINGS",
                    "BOT_MESSAGE_1"),
                "1"),

            new(
                this.GetText(
                    "ADMIN_HOSTSETTINGS",
                    "BOT_MESSAGE_2"),
                "2")
        ];

        this.SendWelcomeNotificationAfterRegisterList = [
            new(
                this.GetText(
                    "ADMIN_HOSTSETTINGS",
                    "WELCOME_NOTIFICATION_0"),
                "0"),

            new(
                this.GetText(
                    "ADMIN_HOSTSETTINGS",
                    "WELCOME_NOTIFICATION_1"),
                "1"),

            new(
                this.GetText(
                    "ADMIN_HOSTSETTINGS",
                    "WELCOME_NOTIFICATION_2"),
                "2")
        ];
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData()
    {
        // load Board Setting collection information...
        var settingCollection = new BoardSettingCollection(this.PageBoardContext.BoardSettings);

        // handle checked fields...
        settingCollection.SettingsBool.Keys.ForEach(
            name =>
            {
                var propertyInfo = this.Input.GetType().GetProperty(name);

                if (propertyInfo is null || propertyInfo.PropertyType.Name != "Boolean"
                                         || !settingCollection.SettingsBool[name].CanRead)
                {
                    return;
                }

                // get the value from the property...
                var value = (bool)Convert.ChangeType(
                    settingCollection.SettingsBool[name].GetValue(this.PageBoardContext.BoardSettings, null),
                    typeof(bool));

                propertyInfo.SetValue(this.Input, Convert.ChangeType(value, propertyInfo.PropertyType), null);
            });

        // handle string fields...
        settingCollection.SettingsString.Keys.ForEach(
            name =>
            {
                var propertyInfo = this.Input.GetType().GetProperty(name);

                if (propertyInfo is null || propertyInfo.PropertyType.Name != "String"
                                         || !settingCollection.SettingsString[name].CanRead)
                {
                    return;
                }

                // get the value from the property...
                var value = (string)Convert.ChangeType(
                    settingCollection.SettingsString[name].GetValue(this.PageBoardContext.BoardSettings, null),
                    typeof(string));

                propertyInfo.SetValue(this.Input, Convert.ChangeType(value, propertyInfo.PropertyType), null);
            });

        // handle int fields...
        settingCollection.SettingsInt.Keys.ForEach(
            name =>
            {
                var propertyInfo = this.Input.GetType().GetProperty(name);

                if (propertyInfo is null || !settingCollection.SettingsInt[name].CanRead)
                {
                    return;
                }

                if (propertyInfo.PropertyType.Name == "Int32")
                {
                    // get the value from the property...
                    var value = (string)Convert.ChangeType(
                        settingCollection.SettingsInt[name].GetValue(this.PageBoardContext.BoardSettings, null),
                        typeof(string));

                    propertyInfo.SetValue(this.Input, Convert.ChangeType(value, propertyInfo.PropertyType), null);
                }
                else if (propertyInfo.PropertyType.Name == "String")
                {
                    // get the value from the property...
                    var value = (string)Convert.ChangeType(
                        settingCollection.SettingsInt[name].GetValue(this.PageBoardContext.BoardSettings, null),
                        typeof(string));

                    propertyInfo.SetValue(this.Input, Convert.ChangeType(value, TypeCode.String), null);
                }
            });

        // special field handling...
        this.Input.AvatarSize = this.PageBoardContext.BoardSettings.AvatarSize != 0
                                    ? this.PageBoardContext.BoardSettings.AvatarSize.ToString()
                                    : string.Empty;
        this.Input.MaxFileSize = this.PageBoardContext.BoardSettings.MaxFileSize != 0
                                     ? this.PageBoardContext.BoardSettings.MaxFileSize.ToString()
                                     : string.Empty;

        this.Input.AlbumImagesSizeMax = this.PageBoardContext.BoardSettings.AlbumImagesSizeMax != 0
                                            ? this.PageBoardContext.BoardSettings.AlbumImagesSizeMax.ToString()
                                            : string.Empty;

        this.Input.SQLVersion = this.HtmlEncode(this.Get<IDbAccess>().GetSQLVersion());

        this.Input.AppCores = SystemInfo.Processors;
        this.Input.AppMemory =
            $"{SystemInfo.AllocatedMemory.ToType<long>() / 1000000} MB of {SystemInfo.MappedMemory.ToType<long>() / 1000000} MB";
        this.Input.AppOsName = SystemInfo.VersionString;
        this.Input.AppRuntime = $".NET {Environment.Version}";
    }

    /// <summary>
    /// Checks the cache key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>
    /// The check cache key.
    /// </returns>
    private bool CheckCacheKey(string key)
    {
        return this.Get<IDataCache>()[key] != null;
    }

    /// <summary>
    /// Removes the cache key.
    /// </summary>
    /// <param name="key">The key.</param>
    private void RemoveCacheKey(string key)
    {
        if (this.CheckCacheKey(key))
        {
            this.Get<IDataCache>().Remove(key);
        }
    }
}