using Kopilych.Mobile.View_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Mobile.Views.Data_Template_Selectors
{
    public class PiggyBankStepDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NameStepTemplate { get; set; }
        public DataTemplate DescriptionStepTemplate { get; set; }
        public DataTemplate GoalStepTemplate { get; set; }
        public DataTemplate CurrentBalanceStepTemplate { get; set; }
        public DataTemplate GoalDateStepTemplate { get; set; }
        public DataTemplate SharedStepTemplate { get; set; }
        public DataTemplate TypeStepTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var carouselItem = item as PiggyBankInfoPopupViewModel.CarouselItem;
            if (carouselItem != null)
            {
                var step = carouselItem.CurrentStep;
                if (step.IsNameStep)
                    return NameStepTemplate;
                if (step.IsDescriptionStep)
                    return DescriptionStepTemplate;
                if (step.IsGoalStep)
                    return GoalStepTemplate;
                if (step.IsCurrentBalanceStep)
                    return CurrentBalanceStepTemplate;
                if (step.IsSharedStep)
                    return SharedStepTemplate;
                if (step.IsTypeStep)
                    return TypeStepTemplate;
                if (step.IsGoalDateStep)
                    return GoalDateStepTemplate;
            }

            return new DataTemplate();
        }

    }
}
