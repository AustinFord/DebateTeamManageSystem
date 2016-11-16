﻿<%@ Page Title="Edit" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="DebateTeamManagementSystem.Edit" MaintainScrollPositionOnPostback ="true" %>



<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h3>Edit Team Info: </h3>
    <div class="jumbotron">
        
        
        <asp:Label ID="Label1" runat="server" Text="Please Enter Team Name"></asp:Label>
        <asp:TextBox ID="TextBox1" runat="server" OnTextChanged="TextBox1_TextChanged" Width="162px" MaxLength="45"></asp:TextBox>
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Submit" Width="125px" />
         
    
        <div class="gvclass">
        <asp:GridView runat="server" ID="teamsGrid" 
        ItemType="DebateTeamManagementSystem.Models.Team" DataKeyNames="TeamID" 
        SelectMethod="teamsGrid_GetData"
        UpdateMethod="teamsGrid_UpdateItem"
        DeleteMethod="teamsGrid_DeleteItem"
        AutoGenerateEditButton="True"
        AutoGenerateDeleteButton="True"
        AutoGenerateColumns="False" AllowPaging="True" AllowSorting="True" CellPadding="3" Font-Size="Medium" Height="249px" PageSize="5" Width="522px">
        <Columns>
            <asp:DynamicField DataField="TeamName" HeaderText="Team Name" >
            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"/>
            </asp:DynamicField>
            <asp:DynamicField DataField="isActive" HeaderText="Is Active" >
            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"/>
            </asp:DynamicField>
            
            
        </Columns>
            <EditRowStyle BackColor="Silver" HorizontalAlign="Center" VerticalAlign="Middle" />
            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
            <PagerStyle HorizontalAlign="Center" VerticalAlign="Middle" />
            <RowStyle HorizontalAlign="Center" VerticalAlign="Middle" />
            <SortedAscendingHeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
            <SortedDescendingHeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
    </asp:GridView>

            <asp:PlaceHolder runat="server" ID="TeamError" Visible="false">
                        <p class="text-danger">
                            <asp:Literal runat="server" ID="TeamErrorText" />
                        </p>
                    </asp:PlaceHolder>
        </div>
        <br />
       
         <h3>Please select a season start and end date.</h3>
        <div style ="width: 1000px; height: 30px; margin: 0 auto; column-count:2; display: inline;">
            <div>
                <h4>Start date:</h4>
            <asp:Calendar ID="StartDate" runat="server" BackColor="White" BorderColor="#3366CC" BorderWidth="1px" CellPadding="0" DayNameFormat="Shortest" Font-Names="Verdana" Font-Size="8pt" ForeColor="#003399" Height="16px" Width="220px" OnSelectionChanged="StartDate_SelectionChanged">
                <DayHeaderStyle BackColor="#99CCCC" ForeColor="#336666" Height="1px" />
                <NextPrevStyle Font-Size="8pt" ForeColor="#CCCCFF" />
                <OtherMonthDayStyle ForeColor="#999999" />
                <SelectedDayStyle BackColor="#009999" Font-Bold="True" ForeColor="#CCFF99" />
                <SelectorStyle BackColor="#99CCCC" ForeColor="#336666" />
                <TitleStyle BackColor="#003399" BorderColor="#3366CC" BorderWidth="1px" Font-Bold="True" Font-Size="10pt" ForeColor="#CCCCFF" Height="25px" />
                <TodayDayStyle BackColor="#99CCCC" ForeColor="White" />
                <WeekendDayStyle BackColor="#CCCCFF" />
            </asp:Calendar>
            </div>
            <div>
                <h4>End date:</h4>
            <asp:Calendar ID="EndDate" runat="server" BackColor="White" BorderColor="#3366CC" BorderWidth="1px" CellPadding="0" DayNameFormat="Shortest" Font-Names="Verdana" Font-Size="8pt" ForeColor="#003399" Height="16px" Width="220px" OnSelectionChanged="EndDate_SelectionChanged">
                <DayHeaderStyle BackColor="#99CCCC" ForeColor="#336666" Height="1px" />
                <NextPrevStyle Font-Size="8pt" ForeColor="#CCCCFF" />
                <OtherMonthDayStyle ForeColor="#999999" />
                <SelectedDayStyle BackColor="#009999" Font-Bold="True" ForeColor="#CCFF99" />
                <SelectorStyle BackColor="#99CCCC" ForeColor="#336666" />
                <TitleStyle BackColor="#003399" BorderColor="#3366CC" BorderWidth="1px" Font-Bold="True" Font-Size="10pt" ForeColor="#CCCCFF" Height="25px" />
                <TodayDayStyle BackColor="#99CCCC" ForeColor="White" />
                <WeekendDayStyle BackColor="#CCCCFF" />
            </asp:Calendar>

                <asp:PlaceHolder runat="server" ID="DateErrorMessage" Visible="false">
                        <p class="text-danger">
                            <asp:Literal runat="server" ID="InvalidDateText" />
                        </p>
                    </asp:PlaceHolder>
                </div>
            </div>
          
           
        <br />
        <br />
        <asp:Button ID="Button_GenerateSchedule" runat="server" OnClick="GenerateSchedule" Text="Generate Schedule" Width="201px" Height="38px" />
        
        <asp:CheckBox ID="GenerateNewScheduleCheck" runat="server" Text="Confirm Generation?" Visible ="false" />
            
         <asp:PlaceHolder runat="server" ID="GenerateNewSchedule" Visible="false">
                        <p class="text-danger">
                            <asp:Literal runat="server" ID="GenerateNewScheduleText" />
                        </p>
                    </asp:PlaceHolder>
        <br />

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
        DeleteMethod="scheduleGrid_DeleteItem"
        AutoGenerateEditButton="true"
        AutoGenerateDeleteButton="true"
        AutoGenerateColumns="false" OnSelectedIndexChanged="scheduleGrid_SelectedIndexChanged" CellPadding="3" AllowSorting="True" Font-Size="Medium">
        <Columns>
            <asp:DynamicField DataField="Team1Name" />
            <asp:DynamicField DataField="Team2Name" />
            <asp:DynamicField DataField="Team1Score" />
            <asp:DynamicField DataField="Team2Score" />
            <asp:DynamicField DataField="date" />
            <asp:DynamicField DataField="time" />
            <asp:DynamicField DataField="isLocked" />
             <asp:DynamicField DataField="RoundStatus" />
            
        </Columns>
            <EditRowStyle CssClass="GridViewEditRow" BackColor="Silver" HorizontalAlign="Center" VerticalAlign="Middle" />
            <RowStyle HorizontalAlign="Center" VerticalAlign="Middle" />
    </asp:GridView>

            <asp:CheckBox ID="ConfirmDeletion" runat="server" Text="Confirm Deletion?" Visible ="false" />
            <asp:PlaceHolder runat="server" ID="DeletionWarning" Visible="false">
                        <p class="text-danger">
                            <asp:Literal runat="server" ID="DeletionWarningText" />
                        </p>
                    </asp:PlaceHolder>
            <asp:Button ID="DeleteSchedule" runat="server" Text="Delete Entire Schedule" OnClick="DeleteSchedule_Click" />    
            <br />
            <br />
            <asp:CheckBox ID="FinalizeCheckbox" runat="server" Text="Confirm Finalization?" Visible ="false" />
            <asp:PlaceHolder runat="server" ID="FinalizeWarning" Visible="false">
                        <p class="text-danger">
                            <asp:Literal runat="server" ID="FinalizeScheduleWarningText" />
                        </p>
                    </asp:PlaceHolder>
            
            <asp:Button ID="FinalizeButton" runat="server" OnClick="FinalizeButton_Click" Text="Finalize Season" />
        </div>
        
    </div>
     
        
 </asp:Content>

