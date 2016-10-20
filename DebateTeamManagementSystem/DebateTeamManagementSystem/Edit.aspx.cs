using System;
using System.Collections;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DebateTeamManagementSystem
{
    

    public partial class Edit : Page
    {
        public ArrayList teamList = new ArrayList();
        string teamNameToDB = "";

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void TeamText_TextChanged(object sender, EventArgs e)
        {

        }

        protected void SubmitTeam_Click(object sender, EventArgs e)
        {
            if (Page.IsValid){
                teamNameToDB = TeamText.Text;
                TeamText.Text = "";
                
            }
        }
        }
    }