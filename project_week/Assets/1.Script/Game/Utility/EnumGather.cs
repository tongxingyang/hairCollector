using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace week
{
    public enum SceneNum
    {
        BaseScene,
        LogoScene,
        LobbyScene,
        GameScene
    }

    public enum Windows
    {
        loadScene,
        messagePopup,
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

    #region dataTable
    public enum DataTable { skill, monster, status, boss, stage, enproj, max }
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
        exp,
        appear_term,
        map,
        max
    }

    public enum BossData
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

    public enum StageData
    {
        coin,
        conditionT,
        conditionB,
        max
    }

    public enum ObstacleData
    {
        name,
        map,
        sizex,
        sizey,
        val,
        max
    }

    public enum EnProjData
    {
        name,
        speed,
        att,
        max
    }

    #endregion

    #region gameData

    public enum skills
    {
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

        max
    }

    public enum getSkillList
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

    public enum Boss
    {
        boss_owl,
        boss_bear,
        boss_scarecrow,
        max
    }

    public enum EnShot
    {
        fireball,

        lightning,

        owl_shot,
        bear_shot,
        scarecrow_shot,
        max
    }

    public enum mapObstacle
    {
        bosszone_0,
        bosszone_1,
        bosszone_2,
        map0,
        map1,
        map2,
        map3,
        map4,
        map5,
        ruin0,
        ruin1,
        ruin2,
        max
    }

    public enum obstacleType
    {
        map,
        bosszone,
        ruin,
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

    public enum cointype
    {
        mopCoin,
        extraCoin
    }

    public enum effAni
    {
        attack,
        explosion,
        bossExplosion,
        hail,
        electric,
        mine
    }

    public enum skillNote
    {
        none,
        pass,
        obstacle,
        continuee
    }

    

    public enum obstacleFab
    {
        static_box_0,
        static_box_1,
        static_box_2,
        static_box_3,
        static_box_4,
        static_box_5,
        boss_owl_zone,
        max
    }

    public enum tileType
    { 
        snowFlake,
        Lava
    }

    public enum dmgTxtType
    {
        standard,
        shield,
        heal
    }

    public enum eDeBuff { slow, dotDem }

    public enum season { spring, summer, fall, winter, max }

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