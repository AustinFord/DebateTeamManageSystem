<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="DebateTeamManagementSystem._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <asp:GridView runat="server" ID="teamsGrid"
        ItemType="DebateTeamManagementSystem.Models.Team" DataKeyNames="TeamID" 
        SelectMethod="teamsGrid_GetData"
        AutoGenerateColumns="False" AllowSorting="True" CellPadding="3" Font-Size="Medium">
        
        <Columns>
            <asp:DynamicField DataField="TeamName" />
        </Columns>
    </asp:GridView>

    </div>


    <div class="jumbotron">
        <asp:GridView runat="server" ID="scheduleGrid"
        ItemType="DebateTeamManagementSystem.Models.TimeSlot" DataKeyNames="TimeSlotID" 
        SelectMethod="scheduleGrid_GetData"
        AutoGenerateColumns="False" AllowSorting="True" CellPadding="3" Font-Size="Medium" >
        
        <Columns>
            <asp:DynamicField DataField="Team1Name" />
            <asp:DynamicField DataField="Team2Name" />
            <asp:DynamicField DataField="Team1Score" />
            <asp:DynamicField DataField="Team2Score" />
            <asp:DynamicField DataField="date" />
            <asp:DynamicField DataField="time" />
        </Columns>
    </asp:GridView>

    </div>
           

    <div class="row">
        <!--<div class="col-md-4">
            <h2>Getting started</h2>
            <p>
                ASP.NET Web Forms lets you build dynamic websites using a familiar drag-and-drop, event-driven model.
            A design surface and hundreds of controls and components let you rapidly build sophisticated, powerful UI-driven sites with data access.
            </p>
            <p>
                <a class="btn btn-default" href="http://go.microsoft.com/fwlink/?LinkId=301948">Learn more &raquo;</a>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Get more libraries</h2>
            <p>
                NuGet is a free Visual Studio extension that makes it easy to add, remove, and update libraries and tools in Visual Studio projects.
            </p>
            <p>
                <a class="btn btn-default" href="http://go.microsoft.com/fwlink/?LinkId=301949">Learn more &raquo;</a>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Web Hosting</h2>
            <p>
                You can easily find a web hosting company that offers the right mix of features and price for your applications.
            </p>
            <p>
                <a class="btn btn-default" href="http://go.microsoft.com/fwlink/?LinkId=301950">Learn more &raquo;</a>
            </p>
        </div>
        -->
    </div>

</asp:Content>
