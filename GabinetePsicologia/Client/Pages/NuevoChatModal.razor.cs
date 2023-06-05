using GabinetePsicologia.Client.Services;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace GabinetePsicologia.Client.Pages
{
    public partial class NuevoChatModal
	{
		[Parameter]
		public string id { get; set; }
		[Inject] ChatServices ChatServices { get; set; }
        public List<ChatPerson> LsAllPeople;
		protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
			LsAllPeople = await ChatServices.GetAllPeople(id);
		}

        
        public void no()
        {
            DialogService.Close();

        }
        public void Select(object args)
        {
            if (args != null)
            {
                var id = args.ToString();
                DialogService.Close(LsAllPeople.FirstOrDefault(x => x.Id == id));
            }
        }

    }

}
