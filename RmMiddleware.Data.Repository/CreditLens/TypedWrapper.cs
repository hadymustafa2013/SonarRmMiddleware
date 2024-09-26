using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Data.Repository.CreditLens.Attributes;
using Data.Repository.CreditLens.Models.Model;
using Data.Repository.CreditLens.Models.ReferenceData.Data.Query;
using Data.Repository.CreditLens.Models.ReferenceData.Data.Query.Projection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RmMiddleware.Helpers;
using Attribute = Data.Repository.CreditLens.Models.ReferenceData.Model.Attribute;
using ModelAttribute = Data.Repository.CreditLens.Models.Model.Attribute;
using ViewAttribute = Data.Repository.CreditLens.Models.View.Attribute;
using ViewGroup = Data.Repository.CreditLens.Models.View;
using ReferenceData = Data.Repository.CreditLens.Models.ReferenceData;

namespace Data.Repository.CreditLens;

public class TypedWrapper<T>
{
    private readonly Uri _baseUri;
    public readonly Dictionary<string, string?>? Headers;
    private readonly string _typeName;

    public TypedWrapper(Uri baseUri, string? userName, string? password)
    {
        _baseUri = baseUri;
        Headers = AuthoriseCreditLens.AuthorizeWithCreditLensAndCreateBearerTokenHeadersForApiRequests
            (_baseUri, userName, password);
        _typeName = typeof(T).Name;
    }

    public TypedWrapper(Uri baseUri, Dictionary<string, string?>? headers)
    {
        _baseUri = baseUri;
        _typeName = typeof(T).Name;
        Headers = headers;
    }

    public async Task<List<T>> ReadRefData()
    {
        if (Headers == null) throw new Exception("No authentication headers.");

        var jObject = await HttpClientHelper
            .GetReturnJObject(new Uri(_baseUri,
                $"/api/refData/{_typeName}"), Headers);

        var value = new List<T>();
        var payload = jObject?["payLoad"];
        if (payload == null) return value;

        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var jToken in payload)
        {
            var jTokenCast = jToken.ToObject<T>();

            if (jTokenCast != null) value.Add(jTokenCast);
        }

        return value;
    }

    public async Task<long?> ExistsSearchViewModel(Dictionary<string, object> search)
    {
        if (Headers == null) throw new Exception("No authentication headers.");
        var body = JsonConvert.SerializeObject(new { payLoad = search });

        var jObject = await HttpClientHelper
            .PostStringBodyReturnJObject(new Uri(_baseUri,
                $"api/search/{_typeName}Search"), body, Headers);

        if (jObject?["payLoad"]?.Type == JTokenType.Null)
        {
            return null;
        }

        var idString = jObject?["payLoad"]?[0]?["Id"]?.ToString();
        if (!long.TryParse(idString, out var value))
        {
            return null;
        }

        return value;
    }

    public async Task<List<T>> SearchViewModel(Dictionary<string, object> search)
    {
        if (Headers == null) throw new Exception("No authentication headers.");

        var value = new List<T>();
        var body = JsonConvert.SerializeObject(new { payLoad = search });

        var jObject = await HttpClientHelper
            .PostStringBodyReturnJObject(new Uri(_baseUri,
                $"api/search/{_typeName}Search"), body, Headers);

        if (jObject == null) return value;
        if (jObject["payLoad"] == null) return value;
        var payload = jObject["payLoad"];
        if (payload?.Type == JTokenType.Null) return value;
        if (payload == null) return value;

        var cryptography = new Cryptography<T>(_baseUri, Headers);
        foreach (var type in payload.Select(jToken => jToken.ToObject<T>()).OfType<T>())
        {
            value.Add(await cryptography.DecryptPropertiesWithEncryptAttribute(type));
        }

        return value;
    }

    public async Task<T> Insert(T t)
    {
        var response = await HttpClientHelper
            .PostStringBodyReturnJObject(
                new Uri(_baseUri, $"/api/data/vm/{_typeName}")
                , JsonConvert.SerializeObject(t), Headers);
        return SetIdProperty(t, response);
    }

    public async Task<T> Update(T t)
    {
        var response = await HttpClientHelper
            .PutStringBodyReturnJObject(
                new Uri(_baseUri, $"/api/data/vm/{_typeName}/{GetIdProperty(t)}")
                , JsonConvert.SerializeObject(t), Headers);
        return SetIdProperty(t, response);
    }
    
    public async Task BuildModelAndViews(T t)
    {
        var model = await BuildDomain(t);
        await BuildViews(model.Attribute);
    }

    private async Task BuildViews(List<ModelAttribute>? attributes)
    {
        var requestBase = BuildViewRequestModel(attributes, false);
        await BuildViewRequest(requestBase);

        var requestSearch = BuildViewRequestModel(attributes, true);
        await BuildViewRequest(requestSearch);
    }

    private async Task BuildViewRequest(ViewGroup.Request request)
    {
        await HttpClientHelper
            .PostStringBodyOnly(
                new Uri(_baseUri, $"/api/meta/v1/modules/Tenant/viewModels")
                , JsonConvert.SerializeObject(request), Headers);

        await HttpClientHelper.PutStringBodyOnly(new Uri(_baseUri,
                $"/api/meta/v1/modules/Tenant/viewModels/{request.ViewModel?.Id}/versions/1/activate"), null, Headers
        );
    }

    private ViewGroup.Request BuildViewRequestModel(List<ModelAttribute>? attributes, bool search)
    {
        var view = CreateView();
        if (search)
        {
            view.IsSearch = true;
            view.Id = _typeName + "Search";
            view.Description = _typeName + "Search";

            var groupSearchCriteria = CreateGroup("SearchCriteria", 1);
            MapPropertyToGroupAttributes(attributes, groupSearchCriteria, _typeName);
            view.Group?.Add(groupSearchCriteria);

            var groupSearchResults = CreateGroup("SearchResults", 2);
            MapPropertyToGroupAttributes(attributes, groupSearchResults, _typeName);
            view.Group?.Add(groupSearchResults);
        }
        else
        {
            view.IsSearch = false;
            view.Id = _typeName;
            view.Description = _typeName;

            var groupDefault = CreateGroup("Default", 1);
            MapPropertyToGroupAttributes(attributes, groupDefault, _typeName);
            view.Group?.Add(groupDefault);
        }

        var request = new ViewGroup.Request
        {
            ViewModel = view
        };

        return request;
    }

    private ViewGroup.ViewModel CreateView()
    {
        var view = new ViewGroup.ViewModel()
        {
            Group = new List<ViewGroup.Group>(),
            GroupSpecified = true,
            ToolbarButtonSpecified = false,
            SearchQuerySpecified = false,
            ConditionGroupSpecified = false,
            PrimaryModel = _typeName,
            Type = "Tab",
            ColSpan = 0,
            QueryFilterChainId = null,
            AccessControlChainId = null,
            MaxSearchResultCount = 0,
            PageSize = 50,
            IsInfiniteScroll = false,
            IsSearch = false,
            ViewModelCategory = "Regular",
            ResourceModuleId = null,
            NotesEnabled = false,
            Shared = true,
            AllowDraft = false,
            UseBehavior = false
        };
        return view;
    }

    private static ViewGroup.Group CreateGroup(string groupId, int order)
    {
        var group = new ViewGroup.Group
        {
            GroupSpecified = false,
            Attribute = new List<ViewAttribute>(),
            AttributeSpecified = true,
            ViewModelRefSpecified = false,
            ViewModelWidgetSpecified = false,
            Id = groupId,
            ResourceId = null,
            Type = "Tab",
            Action = null,
            ActionRule = null,
            ActionConfigSetting = null,
            Expression = null,
            ColSpanOverride = 0,
            Order = order,
            Deleted = false,
            Shared = true,
            CloudBehavior = "None"
        };
        return group;
    }

    private async Task<Request> BuildDomain(T t)
    {
        var model = new Request
        {
            Id = _typeName,
            DataProvider = "Postgres",
            Description = _typeName,
            DisableModelEvents = false,
            ExcludeFromReportData = false,
            ExcludeHierarchyFromReportData = false,
            Plural = true,
            ReferenceModule = "Core",
            Root = true,
            Source = _typeName.ToLower(),
            VersionEnable = true,
            Attribute = [],
            ModelRef = [],
            Behavior = []
        };

        if (t != null) //Can surely get this from the class instantiation.  How?
        {
            MapClassDecoration(t, model);
            MapPropertyToModelAttributes(t, model);
        }

        await HttpClientHelper
            .PostStringBodyOnly(
                new Uri(_baseUri, $"/api/meta/v1/modules/Tenant/models")
                , JsonConvert.SerializeObject(model), Headers);

        return model;
    }

    public async Task<ReferenceData.Model.Request> BuildReferenceData(T t)
    {
        var model = new ReferenceData.Model.Request
        {
            Id = _typeName,
            Source = "custom_lookup",
            VersionEnable = false,
            Hierarchy = false,
            DataProvider = "Postgres",
            ModelCategory = "Ref",
            NotesEnabled = false,
            MultiModule = false,
            DisplayAsIcon = false,
            ShowAsPill = false,
            Attribute = []
        };

        if (t != null) //Can surely get this from the class instantiation.  How?
        {
            MapPropertyToReferenceDataAttributes(t, model);
        }

        model.Attribute.Add(new Attribute()
        {
            Id = "Type",
            DataType = "STRING",
            PrimaryField = true,
            KeyGenerationStrategy = "ASSIGNED",
            PrimaryFieldOrder = 1
        });

        model.Attribute.Add(new Attribute()
        {
            Id = "IdInt",
            DataType = "INTEGER"
        });

        await HttpClientHelper
            .PostStringBodyOnly(
                new Uri(_baseUri, $"/api/meta/v1/modules/Tenant/models")
                , JsonConvert.SerializeObject(model), Headers);

        var dataRequest = MapForSecondStageOfReferenceDataCreation(model);

        await HttpClientHelper
            .PostStringBodyOnly(
                new Uri(_baseUri, $"/api/meta/v1/modules/Tenant/references")
                , JsonConvert.SerializeObject(dataRequest), Headers);

        return model;
    }

    private ReferenceData.Data.Request MapForSecondStageOfReferenceDataCreation(ReferenceData.Model.Request model)
    {
        var dataRequest = BuildReferenceDataRequestSecondStage();
        MapFirstRequestAttributesToSecondRequestAttributes(model, dataRequest);
        return dataRequest;
    }

    private static void MapFirstRequestAttributesToSecondRequestAttributes(ReferenceData.Model.Request model,
        ReferenceData.Data.Request? dataRequest)
    {
        var displayStrings = new List<string>();
        if (model.Attribute != null)
            foreach (var attribute in model.Attribute.Where(attribute =>
                         attribute.Id != "Type" && attribute.Id != "IdInt"))
            {
                var attributeNew = new ReferenceData.Data.Attribute
                {
                    FieldId = attribute.Id,
                    DataType = attribute.DataType,
                    Key = attribute.PrimaryField,
                    Levels = null,
                    OperationType = "Create"
                };

                if (attribute.PrimaryField)
                {
                    if (dataRequest?.Query?.Projection?.Key != null)
                    {
                        dataRequest.Query.Projection.Key.ModelFieldId = attribute.Id;
                    }

                    attributeNew.Display = false;
                }
                else
                {
                    if (dataRequest?.Query?.Projection?.Display != null)
                    {
                        if (attribute.Id != null) displayStrings.Add(attribute.Id);
                    }

                    attributeNew.Display = true;
                }

                dataRequest?.Attributes?.Add(attributeNew);
            }

        if (dataRequest?.Query?.Projection?.Display != null)
        {
            dataRequest.Query.Projection.Display.ModelFieldIds = string.Join(",", displayStrings);
        }
    }

    private ReferenceData.Data.Request BuildReferenceDataRequestSecondStage()
    {
        var dataRequest = new ReferenceData.Data.Request()
        {
            Attributes = [],
            ModelBased = false,
            Id = _typeName,
            Description = _typeName,
            Query = new Query
            {
                Projection = new Projection
                {
                    Key = new Key(),
                    Display = new Display()
                }
            },
            ModelRefId = _typeName,
            Type = _typeName.ToUpper()
        };
        return dataRequest;
    }

    private static void MapClassDecoration([DisallowNull] T t, Request model)
    {
        foreach (var attribute in t.GetType().GetCustomAttributes())
        {
            if (attribute.GetType() != typeof(BehaviourAttribute)) continue;

            var castAttribute = (BehaviourAttribute)attribute;
            var behaviour = new Behavior
            {
                Plural = castAttribute.Plural,
                Description = castAttribute.Signature,
                Signature = castAttribute.Signature,
                Type = castAttribute.Type
            };
            model.Behavior?.Add(behaviour);
        }
    }

    private static void MapPropertyToModelAttributes(T t, Request request)
    {
        var properties = t?.GetType().GetProperties();
        if (properties == null) return;
        foreach (var property in properties)
        {
            MapPropertyToAttribute(property, request);
        }
    }

    private static void MapPropertyToReferenceDataAttributes(T t, ReferenceData.Model.Request request)
    {
        var properties = t?.GetType().GetProperties();
        if (properties == null) return;
        foreach (var property in properties)
        {
            MapReferenceDataPropertyToAttribute(property, request);
        }
    }

    private static void MapPropertyToGroupAttributes(List<ModelAttribute>? attributes,
        ViewGroup.Group group, string modelId)
    {
        if (attributes == null) return;
        for (var i = 0; i < attributes.Count; i++)
        {
            MapPropertyToGroupAttribute(attributes.ElementAt(i),
                group, i + 1, modelId);
        }
    }

    private static void MapPropertyToGroupAttribute(ModelAttribute attribute,
        ViewGroup.Group group, int order, string modelId)
    {
        var controlType = attribute.DataType switch
        {
            "DateTime" => "Datepicker",
            "Boolean" => "Checkbox",
            _ => "Text"
        };

        var attributeView = new ViewGroup.Attribute()
        {
            Id = attribute.Id,
            PrimaryField = attribute.PrimaryField,
            ViewModelRefSpecified = false,
            Validator = null,
            RunRuleSpecified = false,
            DependsOnAttributesSpecified = false,
            Width = null,
            Filter = false,
            Hidden = false,
            ResourceId = null,
            CellIcon = null,
            CellTooltip = null,
            HorizontalAlign = null,
            ReadOnly = false,
            ModelId = modelId,
            ModelFieldId = attribute.Id,
            Expression = null,
            Encrypt = false,
            ControlType = controlType,
            LookupDependency = null,
            AllowManualEdit = null,
            Format = null,
            Decimals = null,
            Rounding = null,
            ParentAttributeId = null,
            VmRefId = null,
            DisplayField = false,
            MultiSelectField = false,
            LeafOnly = true,
            Action = null,
            ActionRule = null,
            ActionConfigSetting = null,
            DefaultValue = null,
            ViewLabelFieldId = null,
            NotesEnabled = false,
            Order = order,
            Deleted = false,
            ChildModelIdFilter = null,
            VmRefParam = null,
            SortBy = null,
            Shared = true,
            Editable = true,
            Sortable = true,
            Resizeable = true,
            WrapHeaderDisplay = false,
            PinColumnToLeft = false,
            IsAuthAttribute = false,
            SetDefaultOnly = false,
            LinkType = null,
            LinkCondition = null,
            TargetType = null,
            TargetViewModelId = null,
            TargetGroupId = null,
            TargetRouteId = null,
            ModalTitle = null,
            RefParam = null,
            DisplayAsKey = false,
            ClarifyTextEnabled = false,
            CloudBehavior = false,
            ResourceModuleId = "Tenant"
        };

        group.Attribute?.Add(attributeView);
    }

    private static bool HasIgnoreAttribute(MemberInfo property)
    {
        return property.CustomAttributes.Any(
            customAttribute => typeof(IgnoreAttribute) == customAttribute.AttributeType);
    }

    private static void MapPropertyToAttribute(PropertyInfo property, Request request)
    {
        if (HasIgnoreAttribute(property)) return;

        var attribute = new ModelAttribute
        {
            Id = property.Name,
            AttributeExpression = null,
            Calculated = false,
            Encrypt = false,
            Description = property.Name,
            IsNew = true,
            RefAttribute = [],
            RefModelId = null,
            ResourceLabel = property.Name,
            Transient = null,
            Unique = null,
            PrimaryField = false,
            DataType = GetDataTypeFromPropertyType(property)
        };

        ModifyAttributeGivenDecoration(property, attribute, request);
        request.Attribute?.Add(attribute);
    }

    private static void MapReferenceDataPropertyToAttribute(PropertyInfo property, ReferenceData.Model.Request request)
    {
        if (HasIgnoreAttribute(property)) return;

        var attribute = new Attribute()
        {
            Id = property.Name,
            PrimaryField = false,
            DataType = GetDataTypeFromPropertyType(property)
        };

        ModifyReferenceDataGivenDecoration(property, attribute);
        request.Attribute?.Add(attribute);
    }

    private static void ModifyReferenceDataGivenDecoration(PropertyInfo property, Attribute attribute)
    {
        var decorations = property.GetCustomAttributes(true);
        if (decorations.Length <= 0) return;
        foreach (var decoration in decorations)
        {
            ProcessReferenceDatePrimaryKeyDecoration(decoration, attribute, property);
        }
    }

    private static void ModifyAttributeGivenDecoration(PropertyInfo property, ModelAttribute attribute, Request request)
    {
        var decorations = property.GetCustomAttributes(true);
        if (decorations.Length <= 0) return;
        foreach (var decoration in decorations)
        {
            ProcessPrimaryKeyDecoration(decoration, attribute, property);
            ProcessEncryptionDecoration(decoration, attribute);
            ProcessRelationshipDecoration(decoration, property, request);
            ProcessDataTypeDecoration(decoration, attribute);
        }
    }

    private static void ProcessRelationshipDecoration(object decoration, PropertyInfo property, Request request)
    {
        if (typeof(ModelReferenceAttribute) != decoration.GetType()) return;
        var modelReferenceAttribute = (ModelReferenceAttribute)decoration;
        var modelRef = new ModelRef()
        {
            Embedded = false,
            ModelId = modelReferenceAttribute.OtherModelId,
            Relation = modelReferenceAttribute.Relationship,
            OneToOneRelationType = modelReferenceAttribute.OneToOneRelationType,
            TreatAsOneToOne = false,
            DefaultDesignator = null,
            Description =
                $"{modelReferenceAttribute.OtherModelId} on {modelReferenceAttribute.OtherModelKey}",
            Id = modelReferenceAttribute.OtherModelId,
            RefAttribute = []
        };

        var refAttribute = new RefAttribute
        {
            Id = modelReferenceAttribute.OtherModelKey,
            SourceId = property.Name
        };
        modelRef.RefAttribute.Add(refAttribute);

        request.ModelRef?.Add(modelRef);
    }

    private static void ProcessPrimaryKeyDecoration(object decoration, ModelAttribute attribute, PropertyInfo property)
    {
        if (typeof(PrimaryKeyAttribute) != decoration.GetType()) return;
        var primaryKeyAttribute = (PrimaryKeyAttribute)decoration;
        attribute.Id = property.Name;
        attribute.KeyGenerationStrategy = primaryKeyAttribute.KeyGenerationStrategy;
        attribute.PrimaryField = true;
        attribute.Unique = null;
    }

    private static void ProcessEncryptionDecoration(object decoration, ModelAttribute attribute)
    {
        if (typeof(EncryptAttribute) != decoration.GetType()) return;
        attribute.Encrypt = true;
    }

    private static void ProcessDataTypeDecoration(object decoration, ModelAttribute attribute)
    {
        if (typeof(DataTypeAttribute) != decoration.GetType()) return;
        var primaryKeyAttribute = (DataTypeAttribute)decoration;
        attribute.DataType = primaryKeyAttribute.DateType;
    }

    private static void ProcessReferenceDatePrimaryKeyDecoration(object decoration, Attribute attribute,
        PropertyInfo property)
    {
        if (typeof(PrimaryKeyAttribute) != decoration.GetType()) return;
        var primaryKeyAttribute = (PrimaryKeyAttribute)decoration;
        attribute.Id = property.Name;
        attribute.KeyGenerationStrategy = primaryKeyAttribute.KeyGenerationStrategy;
        attribute.PrimaryField = true;
        attribute.PrimaryFieldOrder = 2;
    }

    private static string GetDataTypeFromPropertyType(PropertyInfo property)
    {
        if (property.PropertyType == typeof(long) || property.PropertyType == typeof(long?))
        {
            return "Long";
        }

        if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?))
        {
            return "Integer";
        }

        if (property.PropertyType == typeof(double) || property.PropertyType == typeof(double?))
        {
            return "Double";
        }

        if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
        {
            return "DateTime";
        }

        if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?))
        {
            return "Boolean";
        }

        if (property.PropertyType == typeof(decimal) || property.PropertyType == typeof(decimal?))
        {
            return "Decimal";
        }

        if (property.PropertyType == typeof(Guid) || property.PropertyType == typeof(Guid?))
        {
            return "Guid";
        }

        return "String";
    }

    private static long? GetIdProperty(T t)
    {
        var id = t?.GetType().GetProperty("Id");
        if (id == null) return null;
        var value = id.GetValue(t, null);
        return (long?)value;
    }

    private static T SetIdProperty(T t, JObject? response)
    {
        var id = t?.GetType().GetProperty("Id");
        if (id != null)
        {
            var idValue = int.Parse(response?["payLoad"]?[0]?["Id"]?.ToString() ?? string.Empty);
            id.SetValue(t, idValue);
        }
        else
        {
            throw new Exception("Type does not have an Id property.");
        }

        return t;
    }
}