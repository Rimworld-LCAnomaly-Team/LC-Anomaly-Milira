using LCAnomalyLibrary.Comp;
using LCAnomalyLibrary.Util;
using RimWorld;
using Verse;

namespace LCAnomalyMilira.Comps
{
    public class CompDarkMilira : LC_CompEntity
    {
        public new CompProperties_DarkMilira Props => (CompProperties_DarkMilira)props;

        #region 研究相关

        public override bool CheckStudierSkillRequire(Pawn studier)
        {
            if (studier.skills.GetSkill(SkillDefOf.Intellectual).Level < 10)
            {
                Log.Message($"工作：{studier.Name}的技能{SkillDefOf.Intellectual.label.Translate()}等级不足10，工作固定无法成功");
                return false;
            }

            return true;
        }

        protected override LC_StudyResult CheckFinalStudyQuality(Pawn studier, EAnomalyWorkType workType)
        {
            //每级智力提供2%成功率，10级智力提供20%成功率
            float successRate_Intellectual = studier.skills.GetSkill(SkillDefOf.Intellectual).Level * 0.02f;
            //叠加基础成功率，此处是50%
            float finalSuccessRate = successRate_Intellectual + Props.studySucessRateBase;

            //本能加+10%成功率，洞察-20%成功率，沟通+20%成功率，压迫-30%成功率
            switch (workType)
            {
                case EAnomalyWorkType.Instinct:
                    finalSuccessRate += 0.1f;
                    break;
                case EAnomalyWorkType.Insight:
                    finalSuccessRate -= 0.2f;
                    break;
                case EAnomalyWorkType.Attachment:
                    finalSuccessRate += 0.2f;
                    break;
                case EAnomalyWorkType.Repression:
                    finalSuccessRate -= 0.3f;
                    break;
            }

            //成功率不能超过90%
            if (finalSuccessRate >= 1f)
                finalSuccessRate = 0.9f;

            return Rand.Chance(finalSuccessRate) ? LC_StudyResult.Good : LC_StudyResult.Normal;
        }

        /// <summary>
        /// 检查是否已在图鉴中被解锁
        /// </summary>
        private void CheckIsDiscovered()
        {
            //Log.Message($"检查图鉴解锁情况，我是 {SelfPawn.def.defName}");

            if (AnomalyUtility.ShouldNotifyCodex((Pawn)parent, EntityDiscoveryType.Unfog, out var entries))
            {
                Find.EntityCodex.SetDiscovered(entries, Defs.PawnKindDefOf.DarkMilira.race, (Pawn)parent);
            }
        }

        #endregion 研究相关

        #region 触发事件

        public override void Notify_Holded()
        {
            CheckIsDiscovered();
        }

        #endregion 触发事件
    }
}