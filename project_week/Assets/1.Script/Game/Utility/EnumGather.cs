using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    #region Game

    public enum SceneNum
    {
        BaseScene,
        LogoScene,
        LobbyScene,
        GameScene
    }

    public enum Windows
    {
        win_loadScene,
        win_message,
        win_purchase,
        win_celebrateEff,
        win_coinAni,
        win_accountList,
        win_serverLoad,

        max
    }

    public enum checker { none, ready, complete }
    public enum currency { coin, gem, ap }

    #endregion

    #region [ nanoo ]

    public enum nanooPost { coin, gem, ap, skin, pack }
    public enum nanooPostMsg { key, message }

    #endregion

    #region [ firebase ]

    public enum analyticsWhere
    {
        store,
        post
    }

    #endregion

    #region [ user Data ]

    public enum mulCoinChkList
    {
        removeAD,
        mul_1st_10p,
        mul_1st_5p,
        mul_1st_3p,
        max
    }
    public enum paymentChkList
    {
        removeAD,
        mul_1st_10p,        
        startPack,
        skinPack,
        miniSet,
        santaSet,
        mul_1st_3p,
    }

    public enum utilityChkList
    {
        freeNickChange,     // 무료 닉변
        change_SecondStatus // 새 능력치로 변경되었는가 여부
    }

    #endregion

    #region dataTable

    public enum DataTable { status, skill, skin, monster, boss, enproj, quest, product, max }
    
    /// <summary> 스탯 </summary>
    public enum StatusData
    {
        name,
        sName,

        origin,
        addition,
        additrate,

        bronze,
        silver,
        gold,
        platinum,

        cost,
        stair,

        max
    }

    public enum statusKeyList
    {
        hp,
        att,
        def,
        hpgen,
        cool,
        exp,
        coin,
        skin,
        max
    }

    /// <summary> 스킬 리스트 </summary>
    public enum SkillKeyList
    {
        HP,
        ATT,
        DEF,
        GEN,
        COOL,
        EXP,
        COIN,
        SIZE,
        HEAL,
        SPD,

        Snowball, // SB_Enhance,
        IcicleSpear, FrostDrill,
        IceFist, IceKnuckle,
        HalfIcicle, SnowDart,
        Hammer,
        GigaDrill,
        Ricoche,

        Iceball, // IB_Enhance,
        Hail, Meteor,
        SnowBomb, SnowMissile,
        Circle,
        Poison, Lightning,
        PoisonBomb, Thuncall,

        IceBat, // Bat_Enhance,
        OpenRoader, IcePowder,
        IceBalt, Recovery,
        Flurry, EyeOfFlurry,
        ColdStorm, RotateStorm,
        LockOn,

        Shield, // SD_Enhance,
        HugeShield, GiantShield,
        ThornShield, ReflectShield,
        LightningShield, ChargeShield,
        Invincible, Absorb,
        Hide, Chill,

        Field,
        SnowStorm, Blizzard,
        SnowFog, WhiteOut,
        Aurora, SubStorm,
        IceAge,

        Summon,
        Pet, Pet2, BPet,
        IceWall, Iceberg, Shard,
        Mine,

        Present,

        max
    }

    /// <summary> 스킬 값 </summary>
    public enum SkillValData
    {
        name,
        rank,
        skill_name,
        max_level,

        inheritType,
        essential,
        choice0,
        choice1,
        essential_Tem,

        shot,
        info,
        explain,        

        val0,        
        val0_increase,
        val0_cal, 
        val1,
        val1_increase,
        val1_cal,

        delay,
        delay_reduce,
        keep,
        keep_increase,
        size_increase,
        count,
        count_increase,
        range,
        max
    }

    /// <summary> 스킨 종류 </summary>
    public enum SkinKeyList
    {
        snowman,

        fireman,
        grassman,
        rockman,
        citrusman,
        bulbman,
        wildman,
        mineman,
        robotman,
        icecreamman,
        goldman,
        angelman,
        squareman,
        spiderman,
        vampireman,
        heroman,
        santaman,
        presentman,
        dragonman,

        max
    }

    /// <summary> 스킨 값 </summary>
    public enum SkinValData
    {
        name,
        skinname,

        enable,

        currency,
        price,

        season,

        typeB,

        typeF,
        Fval0,
        Fval1,

        typeI,
        Ival,

        snowball,

        d_hp,
        d_att,
        d_def,
        d_hpgen,
        d_cool,
        d_exp,
        d_coin,
        d_speed, 

        ex_hp,
        ex_att,
        ex_def,
        ex_hpgen,
        ex_cool,
        ex_exp,
        ex_coin,

        max
    }

    public enum inheritType
    { 
        non,
        over,
        overover,
        overSelect
    }

    public enum Mob
    {
        fire,
        closed,
        ranged,
        hard,

        max
    }

    public enum MonsterData
    {
        name,
        hp,
        att,
        def,
        attspeed,
        speed,
        
        patt,
        pspeed,
        
        coin,
        exp,
        
        appear_term,
        
        spring,
        summer,
        fall,
        winter,

        max
    }

    public enum Boss
    {
        boss_butterfly,
        boss_flower,
        boss_scarecrow,
        boss_owl,
        boss_bear,
        max
    }

    public enum BossValData
    {
        hp,
        att,
        def,
        speed,
        skill0,
        skill1,
        map,
        reward,
        coin,
        max
    }
                
    public enum ShotList
    {
        Snowball,
        Icespear,
        Icefist,
        Halficicle,
        Icedrill,
        Iceknuckle,
        Snowdart,
        Hammer,
        Gigadrill,
        Recoche,
        Snowbullet,
        Icebalt,
        Pet,
        Shard,

        Iceball,
        Snowbomb,
        Snowmissile,
        Poison,
        PoisonBomb,
        Mine,
        Present,

        None
    }

    public enum Stamp
    {
        iceballStamp,
        snowbombStamp,
        hailStamp,
        circleStamp,
        crevassStamp,
        meteorStamp,
        poisonStamp,
        lightningStamp,
        sulfurousStamp,
        thuncallStamp,

        icewallStamp,
        icebergStamp,
        mineStamp
    }

    public enum EnShot
    {
        fireball,
        lightning,
        banana,

        bfly_bgPoison,
        bfly_smPoison,
        flower_thorn,
        flower_mine,
        scarecrow_shot,
        scare_fire,
        owl_shot,
        bear_shot,

        max
    }

    public enum EnProjValData
    {
        name,
        speed,
        att,
        max
    }

    public enum Quest
    {
        day_rein,
        day_skin,
        day_ad,
        time,
        boss,
        artifact,
        ad,
        rein
    }

    public enum QuestValData
    {
        name,
        questType,
        questName,
        currency,
        reward,
        explain,
        val
    }

    public enum productKeyList
    {
        removead,
        bonus_3_0,
        startpack,
        wildskinpack,
        miniset,
        santaset,

        ad_gem,
        s_gem,
        m_gem,
        l_gem,
        s_ap,
        m_ap,
        l_ap,
        s_coin,
        m_coin,
        l_coin
    }

    public enum productValData
    {
        name, 
        disposable,
        pricetype, 
        price,
        removead,
        bonus,
        rate,
        gem, 
        ap, 
        coin, 
        addgem, 
        addap, 
        addcoin, 
        skin, 
        image
    }



    #region [recital Data]

    public enum skinBvalue { mine, wild, rebirth, blood, invincible, light, invSlow, frozen, hero, critical, present, max }
    public enum skinFvalue { mine, wild, rebirth, blood, invincible, snowball, iceHeal, criticDmg, present, max }
    public enum skinIvalue { snowball, mine, present, max }
    public enum snowballType { Citrusbaall, Squareball, Rockball, Presentball, standard }

    #endregion

    #endregion

    #region gameData_sub

    public enum mapObstacle
    {
        bosszone,

        map_0,
        map_1,
        map_2,
        map_3,
        map_4,
        map_5,
        map_6,
        map_7,

        gem_0,
        gem_1,

        ruin0,
        ruin1,
        //ruin2,
        max
    }

    public enum obstacleList
    {
        snowrock0,
        snowrock1,
        snowrock2,
        snowrock3,
        rock0,
        rock1,
        tree,
        skull,
        rib,
        max
    }

    public enum effAni
    {
        attack,
        explosion,
        bossExplosion,
        hail,
        electric,
        mine,
        lava,
        crowfire,
        tel,
        playerhit,
        selfEx,
        storm
    }

    public enum dmgTxtType
    {
        standard,
        shield,
        heal,
        att,
        def,
        poison
    }

    #endregion

    public enum skillNote
    {
        none,
        pass,
        obstacle,
        continuee
    }

    

    #region 유저 데이터

    public enum snowStt { maxHp,    att, def, hpgen, cool, exp, coin, size, heal, speed, max }
    public enum eBuff   { hp,       att, def, hpgen, cool, exp, coin, size, heal, speed, snowball, blind, max }
    public enum dayQuest { rein, skin, ad }

    #endregion

    public enum season { spring, summer, fall, winter, not, max }
    public enum landtem { heal, gem, present, sward }

    #region [Sound]

    public enum BGM { Lobby, Battle }
    public enum SFX 
    {
        bossdie,
        click,
        coin,
        die,
        dmg,
        endie,
        exp,
        shot
    }

    #endregion
}