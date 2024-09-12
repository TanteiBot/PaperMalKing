﻿// <auto-generated />
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using PaperMalKing.Database.Models;
using PaperMalKing.Database.Models.Shikimori;

#pragma warning disable 219, 612, 618
#nullable disable

namespace PaperMalKing.Database.CompiledModels
{
    [EntityFrameworkInternal]
    public partial class CustomUpdateColor1EntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "PaperMalKing.Database.Models.Shikimori.ShikiUser.Colors#CustomUpdateColor",
                typeof(CustomUpdateColor),
                baseEntityType,
                sharedClrType: true,
                propertyCount: 4,
                foreignKeyCount: 1,
                keyCount: 1);

            var shikiUserId = runtimeEntityType.AddProperty(
                "ShikiUserId",
                typeof(uint),
                afterSaveBehavior: PropertySaveBehavior.Throw,
                sentinel: 0u);
            shikiUserId.SetAccessors(
                uint (InternalEntityEntry entry) => (entry.FlaggedAsStoreGenerated(0) ? entry.ReadStoreGeneratedValue<uint>(0) : (entry.FlaggedAsTemporary(0) && entry.ReadShadowValue<uint>(0) == 0U ? entry.ReadTemporaryValue<uint>(0) : entry.ReadShadowValue<uint>(0))),
                uint (InternalEntityEntry entry) => entry.ReadShadowValue<uint>(0),
                uint (InternalEntityEntry entry) => entry.ReadOriginalValue<uint>(shikiUserId, 0),
                uint (InternalEntityEntry entry) => entry.ReadRelationshipSnapshotValue<uint>(shikiUserId, 0),
                object (ValueBuffer valueBuffer) => valueBuffer[0]);
            shikiUserId.SetPropertyIndexes(
                index: 0,
                originalValueIndex: 0,
                shadowIndex: 0,
                relationshipIndex: 0,
                storeGenerationIndex: 0);
            shikiUserId.TypeMapping = UIntTypeMapping.Default.Clone(
                comparer: new ValueComparer<uint>(
                    bool (uint v1, uint v2) => v1 == v2,
                    int (uint v) => ((int)(v)),
                    uint (uint v) => v),
                keyComparer: new ValueComparer<uint>(
                    bool (uint v1, uint v2) => v1 == v2,
                    int (uint v) => ((int)(v)),
                    uint (uint v) => v),
                providerValueComparer: new ValueComparer<uint>(
                    bool (uint v1, uint v2) => v1 == v2,
                    int (uint v) => ((int)(v)),
                    uint (uint v) => v),
                mappingInfo: new RelationalTypeMappingInfo(
                    storeTypeName: "INTEGER"));
            shikiUserId.SetCurrentValueComparer(new EntryCurrentValueComparer<uint>(shikiUserId));

            var id = runtimeEntityType.AddProperty(
                "Id",
                typeof(int),
                valueGenerated: ValueGenerated.OnAdd,
                afterSaveBehavior: PropertySaveBehavior.Throw,
                sentinel: 0);
            id.SetAccessors(
                int (InternalEntityEntry entry) => (entry.FlaggedAsStoreGenerated(1) ? entry.ReadStoreGeneratedValue<int>(1) : (entry.FlaggedAsTemporary(1) && entry.ReadShadowValue<int>(1) == 0 ? entry.ReadTemporaryValue<int>(1) : entry.ReadShadowValue<int>(1))),
                int (InternalEntityEntry entry) => entry.ReadShadowValue<int>(1),
                int (InternalEntityEntry entry) => entry.ReadOriginalValue<int>(id, 1),
                int (InternalEntityEntry entry) => entry.ReadRelationshipSnapshotValue<int>(id, 1),
                object (ValueBuffer valueBuffer) => valueBuffer[1]);
            id.SetPropertyIndexes(
                index: 1,
                originalValueIndex: 1,
                shadowIndex: 1,
                relationshipIndex: 1,
                storeGenerationIndex: 1);
            id.TypeMapping = IntTypeMapping.Default.Clone(
                comparer: new ValueComparer<int>(
                    bool (int v1, int v2) => v1 == v2,
                    int (int v) => v,
                    int (int v) => v),
                keyComparer: new ValueComparer<int>(
                    bool (int v1, int v2) => v1 == v2,
                    int (int v) => v,
                    int (int v) => v),
                providerValueComparer: new ValueComparer<int>(
                    bool (int v1, int v2) => v1 == v2,
                    int (int v) => v,
                    int (int v) => v),
                mappingInfo: new RelationalTypeMappingInfo(
                    storeTypeName: "INTEGER"));
            id.SetCurrentValueComparer(new EntryCurrentValueComparer<int>(id));

            var colorValue = runtimeEntityType.AddProperty(
                "ColorValue",
                typeof(int),
                propertyInfo: typeof(CustomUpdateColor).GetProperty("ColorValue", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CustomUpdateColor).GetField("<ColorValue>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0);
            colorValue.SetGetter(
                int (CustomUpdateColor entity) => CustomUpdateColorUnsafeAccessors.ColorValue(entity),
                bool (CustomUpdateColor entity) => CustomUpdateColorUnsafeAccessors.ColorValue(entity) == 0,
                int (CustomUpdateColor instance) => CustomUpdateColorUnsafeAccessors.ColorValue(instance),
                bool (CustomUpdateColor instance) => CustomUpdateColorUnsafeAccessors.ColorValue(instance) == 0);
            colorValue.SetSetter(
                (CustomUpdateColor entity, int value) => CustomUpdateColorUnsafeAccessors.ColorValue(entity) = value);
            colorValue.SetMaterializationSetter(
                (CustomUpdateColor entity, int value) => CustomUpdateColorUnsafeAccessors.ColorValue(entity) = value);
            colorValue.SetAccessors(
                int (InternalEntityEntry entry) => CustomUpdateColorUnsafeAccessors.ColorValue(((CustomUpdateColor)(entry.Entity))),
                int (InternalEntityEntry entry) => CustomUpdateColorUnsafeAccessors.ColorValue(((CustomUpdateColor)(entry.Entity))),
                int (InternalEntityEntry entry) => entry.ReadOriginalValue<int>(colorValue, 2),
                int (InternalEntityEntry entry) => entry.GetCurrentValue<int>(colorValue),
                object (ValueBuffer valueBuffer) => valueBuffer[2]);
            colorValue.SetPropertyIndexes(
                index: 2,
                originalValueIndex: 2,
                shadowIndex: -1,
                relationshipIndex: -1,
                storeGenerationIndex: -1);
            colorValue.TypeMapping = IntTypeMapping.Default.Clone(
                comparer: new ValueComparer<int>(
                    bool (int v1, int v2) => v1 == v2,
                    int (int v) => v,
                    int (int v) => v),
                keyComparer: new ValueComparer<int>(
                    bool (int v1, int v2) => v1 == v2,
                    int (int v) => v,
                    int (int v) => v),
                providerValueComparer: new ValueComparer<int>(
                    bool (int v1, int v2) => v1 == v2,
                    int (int v) => v,
                    int (int v) => v),
                mappingInfo: new RelationalTypeMappingInfo(
                    storeTypeName: "INTEGER"));

            var updateType = runtimeEntityType.AddProperty(
                "UpdateType",
                typeof(byte),
                propertyInfo: typeof(CustomUpdateColor).GetProperty("UpdateType", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CustomUpdateColor).GetField("<UpdateType>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: (byte)0);
            updateType.SetGetter(
                byte (CustomUpdateColor entity) => CustomUpdateColorUnsafeAccessors.UpdateType(entity),
                bool (CustomUpdateColor entity) => CustomUpdateColorUnsafeAccessors.UpdateType(entity) == 0,
                byte (CustomUpdateColor instance) => CustomUpdateColorUnsafeAccessors.UpdateType(instance),
                bool (CustomUpdateColor instance) => CustomUpdateColorUnsafeAccessors.UpdateType(instance) == 0);
            updateType.SetSetter(
                (CustomUpdateColor entity, byte value) => CustomUpdateColorUnsafeAccessors.UpdateType(entity) = value);
            updateType.SetMaterializationSetter(
                (CustomUpdateColor entity, byte value) => CustomUpdateColorUnsafeAccessors.UpdateType(entity) = value);
            updateType.SetAccessors(
                byte (InternalEntityEntry entry) => CustomUpdateColorUnsafeAccessors.UpdateType(((CustomUpdateColor)(entry.Entity))),
                byte (InternalEntityEntry entry) => CustomUpdateColorUnsafeAccessors.UpdateType(((CustomUpdateColor)(entry.Entity))),
                byte (InternalEntityEntry entry) => entry.ReadOriginalValue<byte>(updateType, 3),
                byte (InternalEntityEntry entry) => entry.GetCurrentValue<byte>(updateType),
                object (ValueBuffer valueBuffer) => valueBuffer[3]);
            updateType.SetPropertyIndexes(
                index: 3,
                originalValueIndex: 3,
                shadowIndex: -1,
                relationshipIndex: -1,
                storeGenerationIndex: -1);
            updateType.TypeMapping = ByteTypeMapping.Default.Clone(
                comparer: new ValueComparer<byte>(
                    bool (byte v1, byte v2) => v1 == v2,
                    int (byte v) => ((int)(v)),
                    byte (byte v) => v),
                keyComparer: new ValueComparer<byte>(
                    bool (byte v1, byte v2) => v1 == v2,
                    int (byte v) => ((int)(v)),
                    byte (byte v) => v),
                providerValueComparer: new ValueComparer<byte>(
                    bool (byte v1, byte v2) => v1 == v2,
                    int (byte v) => ((int)(v)),
                    byte (byte v) => v),
                mappingInfo: new RelationalTypeMappingInfo(
                    storeTypeName: "INTEGER"));

            var key = runtimeEntityType.AddKey(
                new[] { shikiUserId, id });
            runtimeEntityType.SetPrimaryKey(key);

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("ShikiUserId") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("Id") }),
                principalEntityType,
                deleteBehavior: DeleteBehavior.Cascade,
                required: true,
                ownership: true);

            var colors = principalEntityType.AddNavigation("Colors",
                runtimeForeignKey,
                onDependent: false,
                typeof(List<CustomUpdateColor>),
                propertyInfo: typeof(ShikiUser).GetProperty("Colors", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ShikiUser).GetField("<Colors>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                eagerLoaded: true);

            colors.SetGetter(
                List<CustomUpdateColor> (ShikiUser entity) => ShikiUserUnsafeAccessors.Colors(entity),
                bool (ShikiUser entity) => ShikiUserUnsafeAccessors.Colors(entity) == null,
                List<CustomUpdateColor> (ShikiUser instance) => ShikiUserUnsafeAccessors.Colors(instance),
                bool (ShikiUser instance) => ShikiUserUnsafeAccessors.Colors(instance) == null);
            colors.SetSetter(
                (ShikiUser entity, List<CustomUpdateColor> value) => ShikiUserUnsafeAccessors.Colors(entity) = value);
            colors.SetMaterializationSetter(
                (ShikiUser entity, List<CustomUpdateColor> value) => ShikiUserUnsafeAccessors.Colors(entity) = value);
            colors.SetAccessors(
                List<CustomUpdateColor> (InternalEntityEntry entry) => ShikiUserUnsafeAccessors.Colors(((ShikiUser)(entry.Entity))),
                List<CustomUpdateColor> (InternalEntityEntry entry) => ShikiUserUnsafeAccessors.Colors(((ShikiUser)(entry.Entity))),
                null,
                List<CustomUpdateColor> (InternalEntityEntry entry) => entry.GetCurrentValue<List<CustomUpdateColor>>(colors),
                null);
            colors.SetPropertyIndexes(
                index: 1,
                originalValueIndex: -1,
                shadowIndex: -1,
                relationshipIndex: 3,
                storeGenerationIndex: -1);
            colors.SetCollectionAccessor<ShikiUser, List<CustomUpdateColor>, CustomUpdateColor>(
                List<CustomUpdateColor> (ShikiUser entity) => ShikiUserUnsafeAccessors.Colors(entity),
                (ShikiUser entity, List<CustomUpdateColor> collection) => ShikiUserUnsafeAccessors.Colors(entity) = ((List<CustomUpdateColor>)(collection)),
                (ShikiUser entity, List<CustomUpdateColor> collection) => ShikiUserUnsafeAccessors.Colors(entity) = ((List<CustomUpdateColor>)(collection)),
                List<CustomUpdateColor> (ShikiUser entity, Action<ShikiUser, List<CustomUpdateColor>> setter) => ClrCollectionAccessorFactory.CreateAndSet<ShikiUser, List<CustomUpdateColor>, List<CustomUpdateColor>>(entity, setter),
                List<CustomUpdateColor> () => new List<CustomUpdateColor>());
            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            var shikiUserId = runtimeEntityType.FindProperty("ShikiUserId");
            var id = runtimeEntityType.FindProperty("Id");
            var colorValue = runtimeEntityType.FindProperty("ColorValue");
            var updateType = runtimeEntityType.FindProperty("UpdateType");
            var key = runtimeEntityType.FindKey(new[] { shikiUserId, id });
            key.SetPrincipalKeyValueFactory(KeyValueFactoryFactory.CreateCompositeFactory(key));
            key.SetIdentityMapFactory(IdentityMapFactoryFactory.CreateFactory<IReadOnlyList<object>>(key));
            runtimeEntityType.SetOriginalValuesFactory(
                ISnapshot (InternalEntityEntry source) =>
                {
                    var entity = ((CustomUpdateColor)(source.Entity));
                    return ((ISnapshot)(new Snapshot<uint, int, int, byte>(((ValueComparer<uint>)(((IProperty)shikiUserId).GetValueComparer())).Snapshot(source.GetCurrentValue<uint>(shikiUserId)), ((ValueComparer<int>)(((IProperty)id).GetValueComparer())).Snapshot(source.GetCurrentValue<int>(id)), ((ValueComparer<int>)(((IProperty)colorValue).GetValueComparer())).Snapshot(source.GetCurrentValue<int>(colorValue)), ((ValueComparer<byte>)(((IProperty)updateType).GetValueComparer())).Snapshot(source.GetCurrentValue<byte>(updateType)))));
                });
            runtimeEntityType.SetStoreGeneratedValuesFactory(
                ISnapshot () => ((ISnapshot)(new Snapshot<uint, int>(((ValueComparer<uint>)(((IProperty)shikiUserId).GetValueComparer())).Snapshot(default(uint)), ((ValueComparer<int>)(((IProperty)id).GetValueComparer())).Snapshot(default(int))))));
            runtimeEntityType.SetTemporaryValuesFactory(
                ISnapshot (InternalEntityEntry source) => ((ISnapshot)(new Snapshot<uint, int>(default(uint), default(int)))));
            runtimeEntityType.SetShadowValuesFactory(
                ISnapshot (IDictionary<string, object> source) => ((ISnapshot)(new Snapshot<uint, int>((source.ContainsKey("ShikiUserId") ? ((uint)(source["ShikiUserId"])) : 0U), (source.ContainsKey("Id") ? ((int)(source["Id"])) : 0)))));
            runtimeEntityType.SetEmptyShadowValuesFactory(
                ISnapshot () => ((ISnapshot)(new Snapshot<uint, int>(default(uint), default(int)))));
            runtimeEntityType.SetRelationshipSnapshotFactory(
                ISnapshot (InternalEntityEntry source) =>
                {
                    var entity = ((CustomUpdateColor)(source.Entity));
                    return ((ISnapshot)(new Snapshot<uint, int>(((ValueComparer<uint>)(((IProperty)shikiUserId).GetKeyValueComparer())).Snapshot(source.GetCurrentValue<uint>(shikiUserId)), ((ValueComparer<int>)(((IProperty)id).GetKeyValueComparer())).Snapshot(source.GetCurrentValue<int>(id)))));
                });
            runtimeEntityType.Counts = new PropertyCounts(
                propertyCount: 4,
                navigationCount: 0,
                complexPropertyCount: 0,
                originalValueCount: 4,
                shadowCount: 2,
                relationshipCount: 2,
                storeGeneratedCount: 2);
            runtimeEntityType.AddAnnotation("Relational:ContainerColumnName", "Colors");
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "ShikiUsers");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
