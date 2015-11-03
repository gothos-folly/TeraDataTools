using System;
using System.Collections.Generic;
using System.IO;
using DCTools;
using DCTools.Structures;
using Data.Enums;
using Data.Enums.SkillEngine;
using Data.Structures.Player;
using Data.Structures.SkillEngine;
using ProtoBuf;

namespace DataTools.Parsers.DC
{
    class SkillParser
    {
        public static Dictionary<int, Dictionary<int, Dictionary<string, object>>> Strings = new Dictionary<int, Dictionary<int, Dictionary<string, object>>>();

        public static Dictionary<string, Dictionary<string, Animation>> Animations = new Dictionary<string, Dictionary<string, Animation>>();

        public static Dictionary<int, Dictionary<int, UserSkill>> UserSkills = new Dictionary<int, Dictionary<int, UserSkill>>();

        public static Dictionary<int, Dictionary<int, Dictionary<int, Skill>>> Skills = new Dictionary<int, Dictionary<int, Dictionary<int, Skill>>>();

        public static Dictionary<int, DefaultSkillSet> DefaultSkillSets = new Dictionary<int, DefaultSkillSet>();

        public static DataCenter DC = DCT.GetDataCenter();

        public static void Parse()
        {
            LoadStrSheetUserSkill();

            LoadAnimation();

            ParseUserSkills();

            ParseSkills();

            ParseDefaultSkillSets();

            string binPath = Path.GetFullPath("../../../../datapack/gameserver/data/");

            using (FileStream fs = File.Create(binPath + "default_skill_sets.bin"))
            {
                Serializer.Serialize(fs, DefaultSkillSets);
            }

            using (FileStream fs = File.Create(binPath + "user_skills.bin"))
            {
                Serializer.Serialize(fs, UserSkills);
            }

            using (FileStream fs = File.Create(binPath + "skills.bin"))
            {
                Serializer.Serialize(fs, Skills);
            }
        }

        public static void LoadStrSheetUserSkill()
        {
            int count = 0;

            foreach (var o in DC.GetMainObjectsByName("StrSheet_UserSkill"))
            {
                foreach (var data in (List<Dictionary<string, object>>) DC.GetValues(o)["String"])
                {
                    int templateId = int.Parse(data["templateId"].ToString());
                    int id = int.Parse(data["id"].ToString());

                    if (!Strings.ContainsKey(templateId))
                        Strings.Add(templateId, new Dictionary<int, Dictionary<string, object>>());

                    Strings[templateId].Add(id, data);
                    count++;
                }
            }

            Console.WriteLine("Load {0} skill names...", count);
        }

        public static void LoadAnimation()
        {
            int count = 0;

            foreach (var o in DC.GetMainObjectsByName("AnimationData"))
            {
                foreach (var setData in (List<Dictionary<string, object>>) DC.GetValues(o)["AnimSet"])
                {
                    string setName = setData["name"].ToString().ToLower();

                    if (!Animations.ContainsKey(setName))
                        Animations.Add(setName, new Dictionary<string, Animation>());

                    foreach (var data in (List<Dictionary<string, object>>) setData["Animation"])
                    {
                        string name = data["name"].ToString().ToLower();
                        Animation animation = new Animation();

                        animation.Duraction = int.Parse(data["animDuration"].ToString());
                        animation.Dir = int.Parse(data["moveDir"].ToString());

                        for (int i = 0; i < 7; i++)
                            animation.Distance.Add(float.Parse(data["moveDistance" + (i +1)].ToString()));

                        animation.RootMotion = bool.Parse(data["rootMotion"].ToString());
                        animation.RootRotate = bool.Parse(data["rootRotate"].ToString());

                        animation.RotateYaw = int.Parse(data["rotateYaw"].ToString());

                        Animations[setName].Add(name, animation);
                        count++;
                    }
                }
            }

            Console.WriteLine("Load {0} animations...", count);
        }

        public static void ParseUserSkills()
        {
            int count = 0;

            foreach (var o in DC.GetMainObjectsByName("SkillGetConList"))
            {
                foreach (var data in (List<Dictionary<string, object>>)DC.GetValues(o)["SkillList"])
                {
                    UserSkill skill = new UserSkill();

                    skill.SkillId = int.Parse(data["skillId"].ToString());
                    skill.Level = int.Parse(data["level"].ToString());

                    skill.RaceGenderClass
                        = new RaceGenderClass
                              {
                                  Race = (Race) Enum.Parse(typeof (Race), data["race"].ToString()),
                                  Gender = (Gender) Enum.Parse(typeof (Gender), data["gender"].ToString()),
                                  Class = (PlayerClass) Enum.Parse(typeof (PlayerClass), data["class"].ToString()),
                              };

                    skill.IsActive = bool.Parse(data["isActive"].ToString());

                    skill.PrevSkillId = int.Parse(data["prevSkillId"].ToString());
                    skill.PrevSkillOverride = bool.Parse(data["prevSkillOverride"].ToString());

                    if (!UserSkills.ContainsKey(skill.TemplateId))
                        UserSkills.Add(skill.TemplateId, new Dictionary<int, UserSkill>());

                    Dictionary<string, object> stringData;

                    try
                    {
                        stringData = Strings[skill.TemplateId][skill.SkillId];
                    }
                    catch
                    {
                        stringData = null;
                    }

                    if (stringData != null)
                    {
                        skill.Name = stringData["name"].ToString();
                        skill.Tooltip = stringData["tooltip"].ToString();
                    }
                    else
                    {
                        skill.Name = "UNK";
                        skill.Tooltip = "";
                    }

                    UserSkills[skill.TemplateId].Add(skill.SkillId, skill);
                    count++;
                }
            }

            Console.WriteLine("Load {0} user skills...", count);
        }

        public static void ParseSkills()
        {
            int count = 0;

            foreach (var o in DC.GetMainObjectsByName("SkillData"))
            {
                var values = DC.GetValues(o);
                int huntingZoneId = int.Parse(values["huntingZoneId"].ToString());

                if (!values.ContainsKey("Skill"))
                    continue;

                if (!Skills.ContainsKey(huntingZoneId))
                    Skills.Add(huntingZoneId, new Dictionary<int, Dictionary<int, Skill>>());

                foreach (var data in (List<Dictionary<string, object>>) values["Skill"])
                {
                    Skill skill = ParseSkill(huntingZoneId, data);

                    if (!Skills[huntingZoneId].ContainsKey(skill.TemplateId))
                        Skills[huntingZoneId].Add(skill.TemplateId, new Dictionary<int, Skill>());

                    Skills[huntingZoneId][skill.TemplateId].Add(skill.Id, skill);
                    count++;
                }
            }

            Console.WriteLine("Loaded {0} skills...", count);
        }

        protected static Skill ParseSkill(int huntingZoneId, Dictionary<string, object> data)
        {
            Skill skill = new Skill();

            skill.Id = int.Parse(data["id"].ToString());

            if (data.ContainsKey("parentId"))
                skill.ParentId = int.Parse(data["parentId"].ToString());

            skill.Type = (SkillType) Enum.Parse(typeof (SkillType), data["type"].ToString().Replace("_", ""), true);

            skill.Name = data["name"].ToString();

            //category

            skill.TemplateId = int.Parse(data["templateId"].ToString());

            skill.ChangeDirToCenter = bool.Parse(data["changeDirToCenter"].ToString());

            skill.NextSkill = int.Parse(data["nextSkill"].ToString());

            skill.PushTarget = (PushTarget)Enum.Parse(typeof(PushTarget), data["pushtarget"].ToString().Replace("_", ""), true);

            string animSet = data["returnAnimSet"].ToString().ToLower();
            string animName = data["returnAnimName"].ToString().ToLower();

            skill.Animation = animSet.Length > 0 && animName.Length > 0 ? Animations[animSet][animName] : null;

            if (data.ContainsKey("autoUse"))
                skill.AutoUse = bool.Parse(data["autoUse"].ToString());

            if (data.ContainsKey("keepMovingCharge"))
                skill.KeepMovingCharge = bool.Parse(data["keepMovingCharge"].ToString());

            if (data.ContainsKey("keptMovingCharge"))
                skill.KeptMovingCharge = bool.Parse(data["keptMovingCharge"].ToString());

            if (data.ContainsKey("needWeapon"))
                skill.NeedWeapon = bool.Parse(data["needWeapon"].ToString());

            if (data.ContainsKey("timeRate"))
                skill.TimeRate = float.Parse(data["timeRate"].ToString());

            if (data.ContainsKey("totalAtk"))
                skill.TotalAtk = float.Parse(data["totalAtk"].ToString());

            //__value__

            //BattleField

            if (data.ContainsKey("useSkillWhileReaction"))
                skill.UseSkillWhileReaction = bool.Parse(data["useSkillWhileReaction"].ToString());

            if (data.ContainsKey("totalStk"))
                skill.TotalStk = int.Parse(data["totalStk"].ToString());

            if (data.ContainsKey("totalStkPvp"))
                skill.TotalStkPvP = int.Parse(data["totalStkPvp"].ToString());

            //Bullet

            if (data.ContainsKey("Action"))
            {
                List<SkillAction> actions = new List<SkillAction>();

                foreach (var actionData in (List<Dictionary<string, object>>)data["Action"])
                    actions.Add(ParseAction(actionData));

                if (actions.Count > 0)
                    skill.Actions = actions;
            }

            //Defence

            //Drain

            skill.Precondition = ParsePrecondition(((List<Dictionary<string, object>>) data["Precondition"])[0]);

            skill.ProjectileData = ParseProjectileData(((List<Dictionary<string, object>>) data["Projectile"])[0]);

            if (data.ContainsKey("TargetingList"))
            {
                List<Targeting> targetingList = new List<Targeting>();

                foreach (var targetingListData in (List<Dictionary<string, object>>)data["TargetingList"])
                {
                    if (!targetingListData.ContainsKey("Targeting"))
                        continue;

                    foreach (var targetingData in (List<Dictionary<string, object>>) targetingListData["Targeting"])
                        targetingList.Add(ParseTargeting(targetingData));
                }

                if (targetingList.Count > 0)
                    skill.TargetingList = targetingList;
            }

            //Property

            if (data.ContainsKey("ChargingStageList"))
            {
                skill.ChargingStageList = ParseChargingStageList(((List<Dictionary<string, object>>)data["ChargingStageList"])[0]);
            }

            //ConnectPrevSkill

            //Dash

            //Pulling

            //ShortTel

            return skill;
        }

        private static Precondition ParsePrecondition(Dictionary<string, object> data)
        {
            Precondition precondition = new Precondition();

            precondition.CoolTime = int.Parse(data["coolTime"].ToString());

            precondition.ModeChangeMethod = int.Parse(data["modeChangeMethod"].ToString());

            precondition.ModeNo = int.Parse(data["modeNo"].ToString());

            var costData = ((List<Dictionary<string, object>>) data["Cost"])[0];

            precondition.Cost = new Cost
                                    {
                                        Hp = int.Parse(costData["hp"].ToString()),
                                        Mp = int.Parse(costData["mp"].ToString())
                                    };

            precondition.ExclusiveAbnormality = 0;

            return precondition;
        }

        private static ProjectileData ParseProjectileData(Dictionary<string, object> data)
        {
            ProjectileData projectile = new ProjectileData();

            projectile.LifeTime = int.Parse(data["lifeTime"].ToString());

            projectile.AreaBoxSizeX = int.Parse(data["areaBoxSizeX"].ToString());

            projectile.AreaBoxSizeY = int.Parse(data["areaBoxSizeY"].ToString());

            projectile.AreaBoxSizeZ = int.Parse(data["areaBoxSizeZ"].ToString());

            projectile.TargetingList = new List<Targeting>();

            if (data.ContainsKey("TargetingList"))
            {
                foreach (var targetingListData in (List<Dictionary<string, object>>) data["TargetingList"])
                {
                    if (!targetingListData.ContainsKey("Targeting"))
                        continue;

                    foreach (var targetingData in (List<Dictionary<string, object>>) targetingListData["Targeting"])
                        projectile.TargetingList.Add(ParseTargeting(targetingData));
                }
            }

            return projectile;
        }

        private static SkillAction ParseAction(Dictionary<string, object> data)
        {
            SkillAction action = new SkillAction();

            Dictionary<string, object> cancelData = ((List<Dictionary<string, object>>) data["Cancel"])[0];

            action.FrontCancelEndTime = int.Parse(cancelData["frontCancelEndTime"].ToString());

            action.RearCancelStartTime = int.Parse(cancelData["rearCancelStartTime"].ToString());

            if (cancelData.ContainsKey("moveCancelStartTime"))
                action.MoveCancelStartTime = int.Parse(cancelData["moveCancelStartTime"].ToString());

            if (data.ContainsKey("StageList"))
            {
                List<ActionStage> stageList = new List<ActionStage>();

                foreach (var stageListData in (List<Dictionary<string, object>>)data["StageList"])
                {
                    if (!stageListData.ContainsKey("Stage"))
                        continue;

                    foreach (var stageData in (List<Dictionary<string, object>>)stageListData["Stage"])
                        stageList.Add(ParseActionStage(stageData));
                }

                if (stageList.Count > 0)
                    action.StageList = stageList;
            }

            return action;
        }

        private static ActionStage ParseActionStage(Dictionary<string, object> data)
        {
            ActionStage stage = new ActionStage();

            stage.Movable = bool.Parse(data["movable"].ToString());

            if (data.ContainsKey("scriptId"))
                stage.ScriptId = int.Parse(data["scriptId"].ToString());

            if (data.ContainsKey("AnimSeq"))
            {
                stage.AnimationList = new List<AnimSeq>();

                foreach (var animData in (List<Dictionary<string, object>>)data["AnimSeq"])
                    stage.AnimationList.Add(ParseAnimSeq(animData));
            }

            return stage;
        }

        private static AnimSeq ParseAnimSeq(Dictionary<string, object> data)
        {
            AnimSeq anim = new AnimSeq();

            string animSet = data["animSet"].ToString().ToLower();
            string animName = data["animName"].ToString().ToLower().Replace("_(test)", "");

            if (animName.Length > 0)
                if (Animations[animSet].ContainsKey(animName))
                    anim.Animation = Animations[animSet][animName];

            if (data.ContainsKey("movingAnimName"))
            {
                animName = data["movingAnimName"].ToString().ToLower().Replace("_(test)", "");

                if (animName.Length > 0 && Animations[animSet].ContainsKey(animName))
                    anim.MovingAnimation = Animations[animSet][animName];
            }

            if (data.ContainsKey("waitAnimName"))
            {
                animName = data["waitAnimName"].ToString().ToLower().Replace("_(test)", "");

                if (animName.Length > 0 && Animations[animSet].ContainsKey(animName))
                    anim.MovingAnimation = Animations[animSet][animName];
            }

            anim.Duration = int.Parse(data["duration"].ToString());

            anim.BlendInTime = int.Parse(data["blendInTime"].ToString());

            anim.Rate = float.Parse(data["animRate"].ToString());

            anim.Looping = bool.Parse(data["bAnimLooping"].ToString());

            anim.LoopingRate = float.Parse(data["loopingRate"].ToString());

            anim.RootMotionXYRate = float.Parse(data["rootMotionXYRate"].ToString());

            anim.RootMotionZRate = float.Parse(data["rootMotionZRate"].ToString());

            anim.MotionId = int.Parse(data["animMotionId"].ToString());

            return anim;
        }

        private static Targeting ParseTargeting(Dictionary<string, object> data)
        {
            Targeting targeting = new Targeting();

            if (data.ContainsKey("id"))
                targeting.Id = int.Parse(data["id"].ToString());

            if (data.ContainsKey("method"))
                targeting.Method = (TargetingMethod) Enum.Parse(
                    typeof (TargetingMethod), data["method"].ToString().Replace("_", ""), true);

            if (data.ContainsKey("type"))
            {
                targeting.Type = (TargetingType) Enum.Parse(
                    typeof (TargetingType), data["type"].ToString().Replace("_", ""), true);
            }

            if (data.ContainsKey("time"))
                targeting.Time = int.Parse(data["time"].ToString());

            if (data.ContainsKey("startTime"))
                targeting.StartTime = int.Parse(data["startTime"].ToString());

            if (data.ContainsKey("endTime"))
                targeting.EndTime = int.Parse(data["endTime"].ToString());

            if (data.ContainsKey("interval"))
                targeting.Interval = int.Parse(data["interval"].ToString());

            if (data.ContainsKey("until"))
                targeting.Until = int.Parse(data["until"].ToString());

            //__value__

            targeting.AreaList = new List<TargetingArea>();

            if (data.ContainsKey("AreaList"))
            {
                foreach (var targetingAreaListData in (List<Dictionary<string, object>>) data["AreaList"])
                {
                    if (targetingAreaListData.Count < 1)
                        continue;

                    foreach (var targetingAreaData in (List<Dictionary<string, object>>) targetingAreaListData["Area"])
                        targeting.AreaList.Add(ParseTargetingArea(targetingAreaData));
                }
            }

            if (data.ContainsKey("Cost"))
            {
                foreach (var costData in (List<Dictionary<string, object>>) data["Cost"])
                {
                    targeting.Cost = new Cost
                                         {
                                             Hp = int.Parse(costData["hp"].ToString()),
                                             Mp = int.Parse(costData["mp"].ToString())
                                         };

                    break;
                }
            }

            targeting.ProjectileSkillList = new List<ProjectileSkill>();

            if (data.ContainsKey("ProjectileSkillList"))
            {
                foreach (var pslData in (List<Dictionary<string, object>>) data["ProjectileSkillList"])
                {
                    if (!pslData.ContainsKey("ProjectileSkill"))
                        continue;

                    foreach (var psData in (List<Dictionary<string, object>>) pslData["ProjectileSkill"])
                    {
                        targeting.ProjectileSkillList.Add(ParseProjectileSkill(psData));
                    }
                }
            }

            return targeting;
        }

        private static ProjectileSkill ParseProjectileSkill(Dictionary<string, object> data)
        {
            ProjectileSkill skill = new ProjectileSkill();

            skill.Id = int.Parse(data["id"].ToString());

            skill.DetachAngle = int.Parse(data["detachAngle"].ToString());

            skill.DetachDistance = int.Parse(data["detachDistance"].ToString());

            skill.DetachHeight = int.Parse(data["detachHeight"].ToString());

            skill.FlyingDistance = float.Parse(data["flyingDistance"].ToString());

            if (data.ContainsKey("shootAngle"))
                skill.ShootAngle = int.Parse(data["shootAngle"].ToString());

            return skill;
        }

        private static TargetingArea ParseTargetingArea(Dictionary<string, object> data)
        {
            TargetingArea area = new TargetingArea();

            if (data.ContainsKey("type"))
                area.Type = (TargetingAreaType) Enum.Parse(
                    typeof (TargetingAreaType), data["type"].ToString().Replace("_", ""), true);
            else
                area.Type = TargetingAreaType.EnemyOrPvP;

            if (data.ContainsKey("rotateAngle"))
                area.RotateAngle = float.Parse(data["rotateAngle"].ToString());

            if (data.ContainsKey("maxHeight"))
                area.MaxHeight = float.Parse(data["maxHeight"].ToString());

            if (data.ContainsKey("crosshairRadius"))
                area.CrosshairRadius = int.Parse(data["crosshairRadius"].ToString());

            if (data.ContainsKey("maxCount"))
                area.MaxCount = int.Parse(data["maxCount"].ToString());

            if (data.ContainsKey("maxRadius"))
                area.MaxRadius = float.Parse(data["maxRadius"].ToString());

            if (data.ContainsKey("minHeight"))
                area.MinHeight = float.Parse(data["minHeight"].ToString());

            if (data.ContainsKey("minRadius"))
                area.MinRadius = float.Parse(data["minRadius"].ToString());

            if (data.ContainsKey("offsetAngle"))
                area.OffsetAngle = float.Parse(data["offsetAngle"].ToString());

            if (data.ContainsKey("offsetDistance"))
                area.OffsetDistance = float.Parse(data["offsetDistance"].ToString());

            if (data.ContainsKey("pierceDepth"))
                area.PierceDepth = int.Parse(data["pierceDepth"].ToString());

            if (data.ContainsKey("rangeAngle"))
                area.RangeAngle = float.Parse(data["rangeAngle"].ToString());

            if (data.ContainsKey("crosshairRadius2"))
                area.CrosshairRadius2 = int.Parse(data["crosshairRadius2"].ToString());

            area.Effect = ParseAreaEffect(((List<Dictionary<string, object>>) data["Effect"])[0]);

            //HitEffect

            if (data.ContainsKey("Reaction"))
            {
                Dictionary<string, object> reactionData = ((List<Dictionary<string, object>>) data["Reaction"])[0];

                area.ReactionBasicRate = float.Parse(reactionData["basicRate"].ToString());
                area.ReactionMiniRate = float.Parse(reactionData["miniRate"].ToString());
            }

            return area;
        }

        private static AreaEffect ParseAreaEffect(Dictionary<string, object> data)
        {
            AreaEffect effect = new AreaEffect();

            if (data.ContainsKey("id"))
                effect.Id = int.Parse(data["id"].ToString());

            if (data.ContainsKey("posX"))
                effect.PosX = int.Parse(data["posX"].ToString());

            if (data.ContainsKey("posY"))
                effect.PosY = int.Parse(data["posY"].ToString());

            if (data.ContainsKey("posZ"))
                effect.PosZ = int.Parse(data["posZ"].ToString());

            if (data.ContainsKey("startTime"))
                effect.StartTime = int.Parse(data["startTime"].ToString());

            //MoveDistance

            //MoveDuration

            //MovMaxHeight

            if (data.ContainsKey("abnormalityRate"))
                effect.AbnormalityRate = float.Parse(data["abnormalityRate"].ToString());

            if (data.ContainsKey("atk"))
                effect.Atk = float.Parse(data["atk"].ToString());

            if (data.ContainsKey("HpDiff"))
                foreach (var abData in (List<Dictionary<string, object>>)data["HpDiff"]) //0..1
                    effect.HpDiff = int.Parse(abData["value"].ToString());

            if (data.ContainsKey("MpDiff"))
                foreach (var abData in (List<Dictionary<string, object>>)data["MpDiff"]) //0..1
                    effect.MpDiff = int.Parse(abData["value"].ToString());

            //Teleport

            effect.AbnormalityOnCommon = new List<int>();
            if (data.ContainsKey("AbnormalityOnCommon"))
                foreach (var abData in (List<Dictionary<string, object>>)data["AbnormalityOnCommon"]) //0..*
                    effect.AbnormalityOnCommon.Add(int.Parse(abData["id"].ToString()));

            if (data.ContainsKey("AbnormalityOnPvp"))
                foreach (var abData in (List<Dictionary<string, object>>)data["AbnormalityOnPvp"]) //0..1
                    effect.AbnormalityOnPvP = int.Parse(abData["id"].ToString());

            return effect;
        }

        private static ChargingStageList ParseChargingStageList(Dictionary<string, object> data)
        {
            ChargingStageList stageList = new ChargingStageList();

            stageList.Movable = bool.Parse(data["movable"].ToString());

            stageList.CostTotalHp = int.Parse(data["costTotalHp"].ToString());

            stageList.CostTotalMp = int.Parse(data["costTotalMp"].ToString());

            foreach (var stageData in (List<Dictionary<string, object>>)data["ChargeStage"]) //1..*
                stageList.ChargeStageList.Add(ParseChargeStage(stageData));

            var intervalData = ((List<Dictionary<string, object>>) data["Interval"])[0];

            stageList.IntervalCostHpRate = float.Parse(intervalData["costHpRate"].ToString());

            stageList.IntervalCostMpRate = float.Parse(intervalData["costMpRate"].ToString());

            return stageList;
        }

        private static ChargeStage ParseChargeStage(Dictionary<string, object> data)
        {
            ChargeStage stage = new ChargeStage();

            stage.Duration = float.Parse(data["duration"].ToString());

            stage.CostHpRate = float.Parse(data["costHpRate"].ToString());

            stage.CostMpRate = float.Parse(data["costMpRate"].ToString());

            stage.ShotSkillId = int.Parse(data["shotSkillId"].ToString());

            return stage;
        }

        public static void ParseDefaultSkillSets()
        {
            foreach (var o in DC.GetMainObjectsByName("DefaultSkillSet"))
            {
                foreach (var data in (List<Dictionary<string, object>>)DC.GetValues(o)["Default"])
                {
                    DefaultSkillSet set = new DefaultSkillSet();

                    set.RaceGenderClass
                        = new RaceGenderClass
                              {
                                  Race = (Race) Enum.Parse(typeof (Race), data["race"].ToString()),
                                  Gender = (Gender) Enum.Parse(typeof (Gender), data["gender"].ToString()),
                                  Class = (PlayerClass) Enum.Parse(typeof (PlayerClass), data["class"].ToString()),
                              };

                    string[] skills = data["activeSkillIdList"].ToString().Split(';');

                    set.SkillSet = new List<int>();

                    foreach (var skill in skills)
                        set.SkillSet.Add(int.Parse(skill));

                    DefaultSkillSets.Add(set.RaceGenderClass.Hash, set);
                }
            }

            Console.WriteLine("Parsed {0} default skill sets...", DefaultSkillSets.Count);
        }
    }
}


