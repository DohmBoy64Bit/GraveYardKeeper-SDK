using System;
using System.Collections.Generic;
using UnityEngine;

namespace GraveSDK.Data.Repositories
{
    /// <summary>
    /// Advanced Vendor/Economy Repository.
    /// Extends VendorRepository with the ability to create entirely new merchants
    /// from VendorDefinition objects and manipulate economy parameters.
    /// </summary>
    public static class EconomyRepository
    {
        /// <summary>
        /// Creates a new VendorDefinition and registers it in GameBalance.
        /// This allows modders to create entirely new merchants.
        /// </summary>
        public static VendorDefinition CreateVendorDefinition(
            string vendorId,
            float startMoney = 100f,
            int startTier = 1,
            float dailyIncome = 50f,
            List<string> productTypes = null)
        {
            if (GameBalance.me == null) return null;

            var def = new VendorDefinition();
            def.id = vendorId;
            def.start_money = startMoney;
            def.start_tire = startTier;
            def.daily_money_income = dailyIncome;

            if (productTypes != null)
            {
                // product_types is a private field on VendorDefinition, set via reflection
                var field = typeof(VendorDefinition).GetField("product_types",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(def, new List<string>(productTypes));
                }
            }

            // Register in GameBalance
            if (GameBalance.me.vendors_data == null)
                GameBalance.me.vendors_data = new List<VendorDefinition>();

            // Remove existing with same ID
            GameBalance.me.vendors_data.RemoveAll(v => v.id == vendorId);
            GameBalance.me.vendors_data.Add(def);

            Debug.Log("[GraveSDK] Created VendorDefinition: " + vendorId);
            return def;
        }

        /// <summary>
        /// Gets a VendorDefinition by ID from GameBalance.
        /// </summary>
        public static VendorDefinition GetVendorDefinition(string vendorId)
        {
            return GameBalance.me?.GetDataOrNull<VendorDefinition>(vendorId);
        }

        /// <summary>
        /// Gets all vendor instances currently active in the world.
        /// </summary>
        public static List<Vendor> GetAllVendors()
        {
            var result = new List<Vendor>();
            if (WorldMap.objs == null) return result;

            foreach (var wgo in WorldMap.objs)
            {
                if (wgo != null && wgo.vendor != null)
                    result.Add(wgo.vendor);
            }
            return result;
        }

        /// <summary>
        /// Gets a specific active Vendor by its definition ID.
        /// </summary>
        public static Vendor GetVendor(string vendorId)
        {
            if (WorldMap.objs == null) return null;
            foreach (var wgo in WorldMap.objs)
            {
                if (wgo?.vendor != null && wgo.vendor.id == vendorId)
                    return wgo.vendor;
            }
            return null;
        }

        /// <summary>
        /// Sets the money of a vendor.
        /// </summary>
        public static void SetVendorMoney(string vendorId, float money)
        {
            var vendor = GetVendor(vendorId);
            if (vendor != null) vendor.cur_money = money;
        }

        /// <summary>
        /// Adds money to a vendor.
        /// </summary>
        public static void AddVendorMoney(string vendorId, float amount)
        {
            var vendor = GetVendor(vendorId);
            if (vendor != null) vendor.cur_money += amount;
        }

        /// <summary>
        /// Sets the tier of a vendor (1-3).
        /// </summary>
        public static void SetVendorTier(string vendorId, int tier)
        {
            var vendor = GetVendor(vendorId);
            if (vendor != null)
            {
                tier = Mathf.Clamp(tier, 1, 3);
                vendor.cur_tier = tier;
            }
        }

        /// <summary>
        /// Adds an item to a vendor's inventory.
        /// </summary>
        public static void AddItemToVendor(string vendorId, string itemId, int count = 1)
        {
            var vendor = GetVendor(vendorId);
            if (vendor?.inventory != null)
            {
                vendor.inventory.AddItem(itemId, count);
            }
        }

        /// <summary>
        /// Removes an item from a vendor's inventory.
        /// </summary>
        public static void RemoveItemFromVendor(string vendorId, string itemId, int count = 1)
        {
            var vendor = GetVendor(vendorId);
            if (vendor?.inventory != null)
            {
                vendor.inventory.RemoveItem(itemId, count, MultiInventory.DestinationType.AllFromLast);
            }
        }

        /// <summary>
        /// Gets the price a vendor would pay/charge for a specific item.
        /// </summary>
        public static float GetItemPrice(string vendorId, string itemId)
        {
            var vendor = GetVendor(vendorId);
            if (vendor == null) return 0f;

            var itemDef = GameBalance.me?.GetDataOrNull<ItemDefinition>(itemId);
            if (itemDef == null) return 0f;

            return vendor.GetSingleItemPrice(itemDef, 0);
        }

        /// <summary>
        /// Sets the daily income of a vendor definition (affects economy growth).
        /// </summary>
        public static void SetDailyIncome(string vendorId, float dailyIncome)
        {
            var def = GetVendorDefinition(vendorId);
            if (def != null) def.daily_money_income = dailyIncome;
        }

        /// <summary>
        /// Modifies the base_price of an item across the entire economy.
        /// This affects ALL vendors that trade this item.
        /// </summary>
        public static void SetItemBasePrice(string itemId, float newPrice)
        {
            var itemDef = GameBalance.me?.GetDataOrNull<ItemDefinition>(itemId);
            if (itemDef != null)
            {
                itemDef.base_price = newPrice;
            }
        }
    }
}
