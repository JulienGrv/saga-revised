namespace Saga.Enumarations
{
    /// <summary>
    /// Structure of general errors
    /// </summary>
    public enum Generalerror : byte
    {
        /// <summary>
        /// No error
        /// </summary>
        NoError,

        /// <summary>
        /// This item cannot be equipped at your current level. (Gamestring 2000)
        /// </summary>
        LowCharacterLevel,

        /// <summary>
        /// This item cannot be used with this gender. (Gamestring 2001)
        /// </summary>
        WrongGender,

        /// <summary>
        /// This item cannot be used with this race. (Gamestring 2002)
        /// </summary>
        WrongRace,

        /// <summary>
        /// Your STR is to low to equip this item. (Gamestring 2003)
        /// </summary>
        LowStrength,

        /// <summary>
        /// Your DEX is to low to equip this item. (Gamestring 2004)
        /// </summary>
        LOW_DEX,

        /// <summary>
        /// Your INT is to low to equip this item. (Gamestring 2005)
        /// </summary>
        LowIntellect,

        /// <summary>
        /// Your CON is to low to equip this item. (Gamestring 2006)
        /// </summary>
        LowConcentration,

        /// <summary>
        /// Your LUK is to low to equip this item. (Gamestring 2007)
        /// </summary>
        LowLuck,

        /// <summary>
        /// Unable to equip because of Job and/or Job level. (Gamestring 2008)
        /// </summary>
        LowJobLevel,

        /// <summary>
        /// Unable to equip because of Ancestor Stone and/or Ancestor Stone level. (Gamestring 2009)
        /// </summary>
        AnchestorStone,

        /// <summary>
        /// Unable to equip because of Weapon and/or Weapon level. (Gamestring 2010)
        /// </summary>
        LowWeaponLevel,

        /// <summary>
        /// Unable to equip on this location. (Gamestring 2011)
        /// </summary>
        WrongLocation,

        /// <summary>
        /// Unable to equip broken equipment. (Gamestring 2012)
        /// </summary>
        EquipmentBroken,

        /// <summary>
        /// There is not enough space in your inventory for this item. (Gamestring 2013)
        /// </summary>
        LowIventorySpace,

        /// <summary>
        /// Duplicate instance of inventory item detected. (Gamestring 2014)
        /// </summary>
        DuplicateInventoryItem,

        /// <summary>
        /// Inventory item not found. (Gamestring 2015)
        /// </summary>
        InventoryItemNotFound,

        /// <summary>
        /// Not enough space in storage to store the item. (Gamestring 2016)
        /// </summary>
        LowStorageSpace,

        /// <summary>
        /// Identical storage item server index detected. (Gamestring 2017)
        /// </summary>
        DuplicateStorageItem,

        /// <summary>
        /// Storage item not found. (Gamestring 2018)
        /// </summary>
        StorageItemNotFound,

        /// <summary>
        /// Item server index is incorrect. (Gamestring 2019)
        /// </summary>
        WrongServerIndex,

        /// <summary>
        /// Received incorrect ItemID. (Gamestring 2020)
        /// </summary>
        WrongItemId,

        /// <summary>
        /// Item cannot be discarded. (Gamestring 2021)
        /// </summary>
        CannotDiscard,

        /// <summary>
        /// Not the same type of item. (Gamestring 2022)
        /// </summary>
        ItemtypeNotMatch,

        /// <summary>
        /// Moved items have exceeded maximum limit. (Gamestring 2023)
        /// </summary>
        MoveItemLimit,

        /// <summary>
        /// Current value is lower then the used value. (Gamestring 2024)
        /// </summary>
        ValueToLow,

        /// <summary>
        /// Unable to equip due to lack of required skills. (Gamestring 2025)
        /// </summary>
        LackRequiredSkills,

        /// <summary>
        /// Delete selected weapon. (Gamestring 2026)
        /// </summary>
        DeleteSelectedWeapon, //This has some werird dialouge and seemed to affect my weapon

        /// <summary>
        /// Weapon does not exist. (Gamestring 2027)
        /// </summary>
        WeaponNotExists,

        /// <summary>
        /// You don't have enough money. (Gamestring 2028)
        /// </summary>
        NotEnoughMoney,

        /// <summary>
        /// You don't have enough EXP points. (Gamestring 2029)
        /// </summary>
        NotEnoughExperience,

        /// <summary>
        /// Not enough levels to absorb current weapon. (Gamestring 2030)
        /// </summary>
        WeaponAbsorbLowLevel,

        /// <summary>
        /// Select equipped weapon. (Gamestring 2031)
        /// </summary>
        SelectWeapon,

        /// <summary>
        /// You don't have enough money. (Gamestring 2032)
        /// </summary>
        NotEnoughMoneyWithDialog, //Same message but with an alert thing

        /// <summary>
        /// You don't have enough money to repair. (Gamestring 2033)
        /// </summary>
        NotEnoughMoneyToRepair,

        /// <summary>
        /// Your Weapon Level is not high enough. (Gamestring 2034)
        /// </summary>
        LowWeaponLevel2,

        /// <summary>
        /// Weapon type doesn't match. (Gamestring 2035)
        /// </summary>
        WeapontypeMismatch,

        /// <summary>
        /// This item can't be equipped. (Gamestring 2036)
        /// </summary>
        ItemtypeNotEquipable,

        /// <summary>
        /// You don't have enough money to repair. (Gamestring 2037)
        /// </summary>
        NotEnoughMoneyToRepair2,

        /// <summary>
        /// You don't have enough money to store in storage. (Gamestring 2038)
        /// </summary>
        NotEnoughMoneyToStore,

        /// <summary>
        /// You don't have enough money to use the storage. (Gamestring 2039)
        /// </summary>
        NotEnoughMoneyToUseStore,

        /// <summary>
        /// Only nameless weapons can be named. (Gamestring 2040)
        /// </summary>
        WeaponNotNameless,

        /// <summary>
        /// Weapon does not exist. (Gamestring 2041)
        /// </summary>
        WeaponNotExists2,

        /// <summary>
        /// Required conditions not met. (Gamestring 2043)
        /// </summary>
        ConditionsNotMet,

        /// <summary>
        /// You don't have enough skill exp points. (Gamestring 2044)
        /// </summary>
        NotEnoughSkillExperience,

        /// <summary>
        /// No former skill level found. (Gamestring 2045)
        /// </summary>
        PreviousSkillNotFound,

        /// <summary>
        /// You already learned that skill. (Gamestring 2047)
        /// </summary>
        AlreadyLearntSkill,

        /// <summary>
        /// You already have this map.(Gamestring 2048)
        /// </summary>
        AlreadyHaveMap,

        /// <summary>
        /// This name cannot be used on the current weapon.(Gamestring 2049)
        /// </summary>
        NameCannotBeUsed,

        /// <summary>
        /// You don't have enough money to change the weapon name.(Gamestring 2050)
        /// </summary>
        NotEnoughMoneyToRenameWeapon,

        /// <summary>
        /// Cannot take off backpack because it contains items.(Gamestring 2051)
        /// </summary>
        BackpackContainsItems,

        /// <summary>
        /// All weapon slots are open. (Gamestring 2052)
        /// </summary>
        AllWeaponSlotsOpen,

        /// <summary>
        /// You cannot enchant this item. (Gamestring 2053)
        /// </summary>
        CannotEnchantItem,

        /// <summary>
        /// Item enchanting failed. (Gamestring 2054)
        /// </summary>
        ItemEnchantmentFailed,

        /// <summary>
        /// Level limit reached. (Gamestring 2055)
        /// </summary>
        LevelLimitReached
    }
}