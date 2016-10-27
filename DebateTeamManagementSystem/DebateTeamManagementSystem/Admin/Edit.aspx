﻿<%@ Page Title="Edit" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="DebateTeamManagementSystem.Edit" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %></h2>
    <h3>Edit Team Info: </h3>
    <div class="jumbotron">
        <asp:Label ID="Label1" runat="server" Text="Please Enter Team Name"></asp:Label>
        <asp:TextBox ID="TextBox1" runat="server" OnTextChanged="TextBox1_TextChanged"></asp:TextBox>
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Submit" />
        <asp:GridView runat="server" ID="teamsGrid"
        ItemType="DebateTeamManagementSystem.Models.Team" DataKeyNames="TeamID" 
        SelectMethod="teamsGrid_GetData"
        UpdateMethod="teamsGrid_UpdateItem"
        DeleteMethod="teamsGrid_DeleteItem"
        AutoGenerateEditButton="true"
        AutoGenerateDeleteButton="true"
        AutoGenerateColumns="false">
        <Columns>
            <asp:DynamicField DataField="TeamName" />
        </Columns>
    </asp:GridView>
        <br />
        <asp:GridView runat="server" ID="scheduleGrid"
        ItemType="DebateTeamManagementSystem.Models.TimeSlot" DataKeyNames="TimeSlotID" 
        SelectMethod="scheduleGrid_GetData"
        UpdateMethod="scheduleGrid_UpdateItem"
        DeleteMethod="scheduleGrid_DeleteItem"
        AutoGenerateEditButton="true"
        AutoGenerateDeleteButton="true"
        AutoGenerateColumns="false" OnSelectedIndexChanged="scheduleGrid_SelectedIndexChanged">
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
</asp:Content>
