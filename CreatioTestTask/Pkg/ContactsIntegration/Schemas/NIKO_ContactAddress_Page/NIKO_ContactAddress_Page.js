define("NIKO_ContactAddress_Page", ["NIKO_ConstValues", "NIKO_ValidatorsHelper"], function(NIKO_ConstValues, NIKO_ValidatorsHelper) {
	return {
		entitySchemaName: "NIKO_ContactAddress",
		attributes: {},
		modules: /**SCHEMA_MODULES*/{}/**SCHEMA_MODULES*/,
		details: /**SCHEMA_DETAILS*/{}/**SCHEMA_DETAILS*/,
		businessRules: /**SCHEMA_BUSINESS_RULES*/{}/**SCHEMA_BUSINESS_RULES*/,
		methods: {
			/* Конфигурация валидатора. */
			setValidationConfig: function() {
                this.callParent(arguments);
				
				this.addColumnValidator("NIKO_Phone", this.phoneValidator);
				this.addColumnValidator("NIKO_Email", this.emailValidator);			
            },		
			
			phoneValidator: function() {
				let email = Ext.isEmpty(this.get("NIKO_Email")) ? null : this.get("NIKO_Email");
            	let phone = Ext.isEmpty(this.get("NIKO_Phone")) ? null : this.get("NIKO_Phone");
				
				return NIKO_ValidatorsHelper.phoneValidator(email, phone);
			},
			
			emailValidator: function() {			
				let email = Ext.isEmpty(this.get("NIKO_Email")) ? null : this.get("NIKO_Email");
            	let phone = Ext.isEmpty(this.get("NIKO_Phone")) ? null : this.get("NIKO_Phone");
				
				return NIKO_ValidatorsHelper.emailValidator(email, phone);
			},
		},
		dataModels: /**SCHEMA_DATA_MODELS*/{}/**SCHEMA_DATA_MODELS*/,
		diff: /**SCHEMA_DIFF*/[
			{
				"operation": "insert",
				"name": "NIKO_AddressType6a79aa34-ad63-4b11-ab45-c1c2b1b419a5",
				"values": {
					"layout": {
						"colSpan": 24,
						"rowSpan": 1,
						"column": 0,
						"row": 0,
						"layoutName": "ProfileContainer"
					},
					"bindTo": "NIKO_AddressType"
				},
				"parentName": "ProfileContainer",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "NIKO_IsActived6e2f6a6-896f-4d91-b9ed-73eef6729e97",
				"values": {
					"layout": {
						"colSpan": 24,
						"rowSpan": 1,
						"column": 0,
						"row": 1,
						"layoutName": "ProfileContainer"
					},
					"bindTo": "NIKO_IsActive"
				},
				"parentName": "ProfileContainer",
				"propertyName": "items",
				"index": 1
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
				"index": 0
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
				"index": 1
			},
			{
				"operation": "insert",
				"name": "NIKO_Homedbc25b0b-fcd2-4a2b-8fea-edffb3c5c7c9",
				"values": {
					"layout": {
						"colSpan": 12,
						"rowSpan": 1,
						"column": 12,
						"row": 1,
						"layoutName": "Header"
					},
					"bindTo": "NIKO_Home"
				},
				"parentName": "Header",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "NIKO_City0258a31f-757c-4241-960e-c3fd4e1a25eb",
				"values": {
					"layout": {
						"colSpan": 12,
						"rowSpan": 1,
						"column": 0,
						"row": 1,
						"layoutName": "Header"
					},
					"bindTo": "NIKO_City"
				},
				"parentName": "Header",
				"propertyName": "items",
				"index": 3
			}
		]/**SCHEMA_DIFF*/
	};
});
