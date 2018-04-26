using System.Collections.Generic;
using Newtonsoft.Json;
using Sitecore.Configuration;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Security;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.ExperienceEditor;
using Sitecore.ExperienceEditor.Speak.Server.Contexts;
using Sitecore.ExperienceEditor.Speak.Server.Requests;
using Sitecore.ExperienceEditor.Speak.Server.Responses;
using Sitecore.ExperienceEditor.Utils;
using Sitecore.Jobs;

namespace Sitecore.Support.ExperienceEditor.Speak.Ribbon.Requests.MyItems
{
	/// <summary>
	/// Custom context class: utilizes refreshIndex variable assigned in Lock.js
	/// </summary>
  public class ItemContextExtended : ItemContext
  {
		[JsonIgnore]
    private bool _refreshIndex;

    [JsonProperty("refreshIndex")]
    public bool RefreshIndex
    {
      get { return _refreshIndex; }
      set { _refreshIndex = value; }
    }

  }

	/// <summary>
	/// Patched method: makes sure index is only refreshed when item is locked
	/// </summary>
  public class MyItemsCountRequest : PipelineProcessorRequest<ItemContext>
  {
    public override PipelineProcessorResponseValue ProcessRequest()
    {
      Assert.IsNotNull(base.RequestContext.Database, string.Format("Could not get context.Database for requestArgs:{0}", base.Args.Data));
      Assert.IsNotNull(Factory.GetDatabase(base.RequestContext.Database), string.Format("Could not get database, with name:{0}", base.RequestContext.Database));

      ItemContextExtended context = JsonConvert.DeserializeObject<ItemContextExtended>(this.Args.Data);

			if (!Sitecore.ExperienceEditor.Settings.WebEdit.ShowNumberOfLockedItemsOnButton)
      {
        return new PipelineProcessorResponseValue
        {
          Value = string.Empty
        };
      }
      if (context.RefreshIndex==true)
      {
        this.RefreshIndex(base.RequestContext.Item);
      }

      return new PipelineProcessorResponseValue
      {
        Value = ItemUtility.GetLockedItemsCountOfCurrentUser()
      };
    }

    protected virtual void RefreshIndex(Item item)
    {
      var index = ContentSearchManager.GetIndex(new SitecoreIndexableItem(item));
      index.Refresh(new SitecoreIndexableItem(item), IndexingOptions.ForcedIndexing);
    }
  }


}