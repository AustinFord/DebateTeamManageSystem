<%@ Page Title="Edit" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RefereeEdit.aspx.cs" Inherits="DebateTeamManagementSystem.RefereeEdit" MaintainScrollPositionOnPostback ="true" %>



<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="jumbotron">
      
     
         
        <div class ="jumbotron" style ="width: 1000px; margin: 0 auto;">
            <asp:PlaceHolder runat="server" ID="ScheduleError" Visible="false">
                        <p class="text-danger">
                            <asp:Literal runat="server" ID="ScheduleErrorText" />
                        </p>
                    </asp:PlaceHolder>
        <asp:GridView runat="server" ID="scheduleGrid"
        ItemType="DebateTeamManagementSystem.Models.TimeSlot" DataKeyNames="TimeSlotID" 
        SelectMethod="scheduleGrid_GetData"
        UpdateMethod="scheduleGrid_UpdateItem"
        AutoGenerateEditButton="true"
        AutoGenerateColumns="false" OnSelectedIndexChanged="scheduleGrid_SelectedIndexChanged" CellPadding="3" AllowSorting="True" Font-Size="Medium">
        <Columns>
            <asp:DynamicField DataField="Team1Name" ReadOnly ="true" />
            <asp:DynamicField DataField="Team2Name" ReadOnly = "true" />
            <asp:DynamicField DataField="Team1Score" />
            <asp:DynamicField DataField="Team2Score" />
            <asp:DynamicField DataField="date" ReadOnly = "true" />
            <asp:DynamicField DataField="time" ReadOnly = "true" />
            <asp:DynamicField DataField="isLocked" ReadOnly = "true" />
             <asp:DynamicField DataField="RoundStatus" />
            
        </Columns>
            <EditRowStyle CssClass="GridViewEditRow" BackColor="Silver" HorizontalAlign="Center" VerticalAlign="Middle" />
            <RowStyle HorizontalAlign="Center" VerticalAlign="Middle" />
    </asp:GridView>
     
         
    </div>
        </div>
        
    
     
        
 </asp:Content>

