<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReplaceWordsImport.ascx.cs" Inherits="YAF.Dialogs.ReplaceWordsImport" %>

<div class="modal fade" id="ReplaceWordsImportDialog" tabindex="-1" role="dialog" aria-labelledby="ReplaceWordsImportDialog" aria-hidden="true">
    <div class="modal-dialog" role="document">

                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">
                            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" 
                                LocalizedPage="ADMIN_REPLACEWORDS_IMPORT" />
                        </h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <!-- Modal Content START !-->
                        <h4>
                            <YAF:HelpLabel ID="LocalizedLabel2" runat="server" LocalizedTag="SELECT_IMPORT" LocalizedPage="ADMIN_REPLACEWORDS_IMPORT" />
                        </h4>
                        <p>
                            <input type="file" id="importFile" class="form-control-file" runat="server" />
                        </p>
                        <!-- Modal Content END !-->
                    </div>
                    <div class="modal-footer">
                        <YAF:ThemeButton id="Import" runat="server" OnClick="Import_OnClick" 
                            TextLocalizedTag="ADMIN_REPLACEWORDS_IMPORT" TextLocalizedPage="TITLE"
                            Type="Primary" Icon="upload">
                        </YAF:ThemeButton>
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">
                            <i class="fa fa-times fa-fw"></i>&nbsp;<YAF:LocalizedLabel runat="server" LocalizedTag="CANCEL"></YAF:LocalizedLabel>
                        </button>
                    </div>
                </div>
    </div>
</div>
