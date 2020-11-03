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

        max
    }

    public enum checker { none, ready, complete }
    public enum currency { coin, gem, ap }

    #endregion

    #region [ user Data ]

    public enum mulCoinChkList
    {
        removeAD,
        mul_1st_10p,
        max
    }
    public enum paymentChkList
    {
        removeAD,
        mulCoins,        
        startPack,
        skinPack
    }

    public enum utilityChkList
    {
        freeNickChange,     // 무료 닉변
        isSavedServer       // 서버에 저장 여부
    }

    #endregion

    #region dataTable

    public enum DataTable { status, skill, skin, monster, boss, enproj, quest, product, max }
    
    /// <summary> 스탯 </summary>
    public enum StatusData
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
        hp,
        att,
        def,
        hpgen,
        cool,
        exp,
        size,
        healmount,
        spd,

        snowball,
        icefist,
        icicle,
        halficicle,
        hail,
        icewall,
        icetornado,
        iceage,
        blizzard,
        snowbomb,
        iceshield,

        poison,
        hammer,
        thunder,
        mine,
        blackhole,
        pet,

        slot,

        max
    }

    /// <summary> 스킬 값 </summary>
    public enum SkillValData
    {
        name,
        skill,
        max_level,
        explain,
        information,
        val,
        att,
        att_increase,
        delay,
        delay_reduce,
        keep,
        keep_increase,
        size_increase,
        count,
        count_increase,
        range,
        note,
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
        goldenarmorman,
        angelman,
        squareman,
        spiderman,
        vampireman,
        heroman,
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

    public enum Mob
    {
        mob_fire,

        mob_ant,
        mob_beetle,
        mob_snail,

        mob_flamingo,
        mob_monkey,
        mob_crab,

        mob_dragonfly,
        mob_candle,
        mob_rino,

        mob_deer,
        mob_stick,
        mob_mam,

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
        appear_term,
        map,
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
        bonus,
        startpack,
        skinpack,
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

    public enum skinBvalue { mine, wild, rebirth, blood, invincible, light, invSlow, frozen, hero, critical, max }
    public enum skinFvalue { mine, wild, rebirth, blood, invincible, snowball, iceHeal, criticDmg, max }
    public enum skinIvalue { snowball, mine, max }
    public enum snowballType { standard, citrus, square, rock }

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

        gem_0,
        gem_1,

        ruin0,
        ruin1,
        ruin2,
        max
    }

    #endregion
        
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
        playerhit
    }

    public enum skillNote
    {
        none,
        pass,
        obstacle,
        continuee
    }

    public enum dmgTxtType
    {
        standard,
        shield,
        heal
    }

    #region 유저 데이터

    public enum snowStt { maxHp, att, def, hpgen, cool, exp, size, heal, speed, max }
    public enum eBuff { att, def, hpgen, cool, exp, size, heal, speed, coin, snowball, max }
    public enum dayQuest { rein, skin, ad }

    #endregion

    public enum season { spring, summer, fall, winter, max }
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