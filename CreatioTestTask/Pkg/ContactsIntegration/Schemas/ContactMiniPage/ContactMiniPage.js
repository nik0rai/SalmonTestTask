define("ContactMiniPage", ["NIKO_ValidatorsHelper"], function(NIKO_ValidatorsHelper) {
	return {
		entitySchemaName: "Contact",
		attributes: {},
		modules: /**SCHEMA_MODULES*/{}/**SCHEMA_MODULES*/,
		details: /**SCHEMA_DETAILS*/{}/**SCHEMA_DETAILS*/,
		businessRules: /**SCHEMA_BUSINESS_RULES*/{}/**SCHEMA_BUSINESS_RULES*/,
		methods: {
			/* Конфигурация валидатора. */
			setValidationConfig: function() {
                this.callParent(arguments);
				
                this.addColumnValidator("Email", () => {
					let email = Ext.isEmpty(this.get("Email")) ? null : this.get("Email");
            		let phone = Ext.isEmpty(this.get("Phone")) ? null : this.get("Phone");
					return NIKO_ValidatorsHelper.emailValidator(email, phone);
				});
				
				this.addColumnValidator("Phone", () => {
					let email = Ext.isEmpty(this.get("Email")) ? null : this.get("Email");
            		let phone = Ext.isEmpty(this.get("Phone")) ? null : this.get("Phone");
					return NIKO_ValidatorsHelper.phoneValidator(email, phone);
				});
            },
		},
		dataModels: /**SCHEMA_DATA_MODELS*/{}/**SCHEMA_DATA_MODELS*/,
		diff: /**SCHEMA_DIFF*/[
			{
				"operation": "merge",
				"name": "HeaderContainer",
				"values": {
					"layout": {
						"colSpan": 24,
						"rowSpan": 1,
						"column": 0,
						"row": 0
					}
				}
			},
			{
				"operation": "merge",
				"name": "Name",
				"values": {
					"layout": {
						"colSpan": 24,
						"rowSpan": 1,
						"column": 0,
						"row": 1
					}
				}
			},
			{
				"operation": "insert",
				"name": "BirthDate98c3a3fe-fc0a-407b-90a6-cf56a8d8fbe1",
				"values": {
					"layout": {
						"colSpan": 24,
						"rowSpan": 1,
						"column": 0,
						"row": 2,
						"layoutName": "MiniPage"
					},
					"isMiniPageModelItem": true,
					"visible": {
						"bindTo": "isAddMode"
					},
					"bindTo": "BirthDate"
				},
				"parentName": "MiniPage",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "insert",
				"name": "Phoneb28be02b-c802-4568-b0d0-f6bd51d46280",
				"values": {
					"layout": {
						"colSpan": 24,
						"rowSpan": 1,
						"column": 0,
						"row": 3,
						"layoutName": "MiniPage"
					},
					"isMiniPageModelItem": true,
					"visible": {
						"bindTo": "isAddMode"
					},
					"bindTo": "Phone"
				},
				"parentName": "MiniPage",
				"propertyName": "items",
				"index": 3
			},
			{
				"operation": "merge",
				"name": "Email",
				"values": {
					"layout": {
						"colSpan": 24,
						"rowSpan": 1,
						"column": 0,
						"row": 4
					}
				}
			},
			{
				"operation": "merge",
				"name": "TimezoneMiniContactPage",
				"values": {
					"layout": {
						"colSpan": 24,
						"rowSpan": 1,
						"column": 0,
						"row": 9
					}
				}
			},
			{
				"operation": "move",
				"name": "TimezoneMiniContactPage",
				"parentName": "MiniPage",
				"propertyName": "items",
				"index": 5
			},
			{
				"operation": "merge",
				"name": "JobInfoContainer",
				"values": {
					"layout": {
						"colSpan": 24,
						"rowSpan": 1,
						"column": 0,
						"row": 10
					}
				}
			},
			{
				"operation": "remove",
				"name": "Type"
			},
			{
				"operation": "remove",
				"name": "Account"
			},
			{
				"operation": "remove",
				"name": "JobTitle"
			},
			{
				"operation": "remove",
				"name": "Department"
			},
			{
				"operation": "remove",
				"name": "MobilePhone"
			},
			{
				"operation": "remove",
				"name": "OwnerEdit"
			},
			{
				"operation": "remove",
				"name": "Owner"
			},
			{
				"operation": "remove",
				"name": "OwnerButtonContainer"
			},
			{
				"operation": "remove",
				"name": "OwnerCallButton"
			},
			{
				"operation": "remove",
				"name": "OwnerEmailButton"
			}
		]/**SCHEMA_DIFF*/
	};
});
