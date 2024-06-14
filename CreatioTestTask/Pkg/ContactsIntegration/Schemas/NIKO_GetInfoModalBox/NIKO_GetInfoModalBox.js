 define("NIKO_GetInfoModalBox", ["ModalBox", "BaseSchemaModuleV2"],
		function(ModalBox) {
	Ext.define("Terrasoft.configuration.NIKO_GetInfoModalBox", {
		extend: "Terrasoft.BaseSchemaModule",
		alternateClassName: "Terrasoft.NIKO_GetInfoModalBox",
		generateViewContainerId: false,
		initSchemaName: function() {
			this.schemaName = "NIKO_GetInfoModalBoxPage";
		},
		initHistoryState: Terrasoft.emptyFn,		
	});
	return Terrasoft.NIKO_GetInfoModalBox;
});