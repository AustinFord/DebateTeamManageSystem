<%@ Page Title="Edit" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="DebateTeamManagementSystem.Edit" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %></h2>
    <h3>Enter Team Names: </h3>
    <p>Use this area to provide additional information.</p>
    <p><asp:Label ID="EnterTeamLabel" runat="server" Text="Enter Team Name: "> </asp:Label><asp:TextBox ID="TeamText" runat="server" OnTextChanged="TeamText_TextChanged"> </asp:TextBox><asp:Button ID="SubmitTeam" runat="server" Text="Submit" OnClick="SubmitTeam_Click" /></p>
    <p><asp:Button ID="DisplayTeams" runat="server" Text="Display Teams" OnClick="DisplayTeams_Click" /></p>
    <p>
    <asp:Panel ID="Panel1" runat="server"></asp:Panel>
</p>
</asp:Content>
