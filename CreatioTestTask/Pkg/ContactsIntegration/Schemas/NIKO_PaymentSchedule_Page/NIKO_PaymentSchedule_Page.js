define("NIKO_PaymentSchedule_Page", ["NIKO_ConstValues", "NIKO_ValidatorsHelper"], function(NIKO_ConstValues, NIKO_ValidatorsHelper) {
	return {
		entitySchemaName: "NIKO_PaymentSchedule",
		attributes: {},
		modules: /**SCHEMA_MODULES*/{}/**SCHEMA_MODULES*/,
		details: /**SCHEMA_DETAILS*/{}/**SCHEMA_DETAILS*/,
		businessRules: /**SCHEMA_BUSINESS_RULES*/{}/**SCHEMA_BUSINESS_RULES*/,
		methods: {
			/* Конфигурация валидатора. */
			setValidationConfig: function() {
                this.callParent(arguments);
				
				this.addColumnValidator("NIKO_Email", () => {
					let email = Ext.isEmpty(this.get("NIKO_Email")) ? null : this.get("NIKO_Email");
            		let phone = Ext.isEmpty(this.get("NIKO_Phone")) ? null : this.get("NIKO_Phone");
					return NIKO_ValidatorsHelper.emailValidator(email, phone);
				});
				
				 this.addColumnValidator("NIKO_Phone", () => {
					let email = Ext.isEmpty(this.get("NIKO_Email")) ? null : this.get("NIKO_Email");
            		let phone = Ext.isEmpty(this.get("NIKO_Phone")) ? null : this.get("NIKO_Phone");
					return NIKO_ValidatorsHelper.phoneValidator(email, phone);
				});
            },		
		},
		dataModels: /**SCHEMA_DATA_MODELS*/{}/**SCHEMA_DATA_MODELS*/,
		diff: /**SCHEMA_DIFF*/[
			{
				"operation": "insert",
				"name": "NIKO_PaymentDateTime6152c98f-61c9-40a0-8c73-256083a7976f",
				"values": {
					"layout": {
						"colSpan": 24,
						"rowSpan": 1,
						"column": 0,
						"row": 0,
						"layoutName": "ProfileContainer"
					},
					"bindTo": "NIKO_PaymentDateTime"
				},
				"parentName": "ProfileContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "NIKO_Sum290314db-cf25-4735-a22b-71ac3559b911",
				"values": {
					"layout": {
						"colSpan": 24,
						"rowSpan": 1,
						"column": 0,
						"row": 1,
						"layoutName": "ProfileContainer"
					},
					"bindTo": "NIKO_Sum"
				},
				"parentName": "ProfileContainer",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "NIKO_Phone7f62fa7b-52a3-4d2c-b62a-b441f6e9cf1e",
				"values": {
					"layout": {
						"colSpan": 12,
						"rowSpan": 1,
						"column": 0,
						"row": 0,
						"layoutName": "Header"
					},
					"bindTo": "NIKO_Phone"
				},
				"parentName": "Header",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "NIKO_Email9935f1b2-acc7-46ca-83c0-cfe48feec027",
				"values": {
					"layout": {
						"colSpan": 12,
						"rowSpan": 1,
						"column": 12,
						"row": 0,
						"layoutName": "Header"
					},
					"bindTo": "NIKO_Email"
				},
				"parentName": "Header",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "NIKO_PaymentPurpose4d8929f0-4fb5-475c-bd7e-95c1203b335c",
				"values": {
					"layout": {
						"colSpan": 12,
						"rowSpan": 1,
						"column": 0,
						"row": 1,
						"layoutName": "Header"
					},
					"bindTo": "NIKO_PaymentPurpose"
				},
				"parentName": "Header",
				"propertyName": "items",
				"index": 2
			}
		]/**SCHEMA_DIFF*/
	};
});
