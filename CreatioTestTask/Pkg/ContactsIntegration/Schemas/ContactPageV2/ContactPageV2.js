define("ContactPageV2", ["ContactDuplicateSearchMixin", "NIKO_ValidatorsHelper", "ModalBox", "ServiceHelper"], 
	   function(ContactDuplicateSearchMixin, NIKO_ValidatorsHelper, ModalBox, ServiceHelper) {
	return {
		entitySchemaName: "Contact",
		details: /**SCHEMA_DETAILS*/{
			"NIKO_PaymentSchedule_Detail": {
				"schemaName": "NIKO_PaymentSchedule_Detail",
				"entitySchemaName": "NIKO_PaymentSchedule",
				"filter": {
					"detailColumn": "NIKO_Contact",
					"masterColumn": "Id"
				}
			},
			"NIKO_ContactAddress_Detail": {
				"schemaName": "NIKO_ContactAddress_Detail",
				"entitySchemaName": "NIKO_ContactAddress",
				"filter": {
					"detailColumn": "NIKO_Contact",
					"masterColumn": "Id"
				}
			}
		}/**SCHEMA_DETAILS*/,
		attributes: {			
			/* Аттрибут, который показывает доступность кнопки "GetInfoFromContact" зависит от "NIKO_IsActive". */
			"GetInfoFromContactEnabled": {
				"dataValueType": Terrasoft.DataValueType.BOOLEAN,
				"type": Terrasoft.ViewModelColumnType.VIRTUAL_COLUMN,
				"value": false
			},
		},
		mixins: {
			ContactDuplicateSearchMixin: "Terrasoft.ContactDuplicateSearchMixin"
		},
		messages: {
			/* Сообщение в секцию, что IsActive было переключено. */
			"GetInfoFromContactChanged": {
				mode: Terrasoft.MessageMode.PTP,
				direction: Terrasoft.MessageDirectionType.PUBLISH
			},
			/* Данные из модального окна. */
			"DataFromModal": {
				mode: Terrasoft.MessageMode.PTP,
				direction: Terrasoft.MessageDirectionType.SUBSCRIBE
			},
			/* Отправка данных в модальное окно. */
			"LoadFieldValues": {
				mode: Terrasoft.MessageMode.PTP,
				direction: Terrasoft.MessageDirectionType.PUBLISH
			},
			/* Событие загрузки модального окна. */
			"ModalPageLoaded": {
				mode: Terrasoft.MessageMode.PTP,
				direction: Terrasoft.MessageDirectionType.SUBSCRIBE
			},
		},
		methods: {		
			init: function() {
				this.callParent(arguments);
				this.on("change:NIKO_IsActive", this.toggleGetInfoFromContact, this);
			},
			onEntityInitialized: function() {
				this.callParent(arguments);
				this.set("GetInfoFromContactEnabled", this.get("NIKO_IsActive"));			
			},
			subscribeSandboxEvents: function() {
				this.callParent(arguments);		
				
				this.sandbox.subscribe("DataFromModal", (obj) => { this.onGetButtonClick(obj); }, this, [this.sandbox.id + "_" + "NIKO_GetInfoModalBox"]);
				
				this.sandbox.subscribe("ModalPageLoaded", 
					this.sendValuesToModalBox, 
					this, 
					[this.sandbox.id + "_" + "NIKO_GetInfoModalBox"]);
			},
			//#region Filters and Validations functions
			/**
			 * Конфигурация валидатора. 
			 * @overridden
			 */
			setValidationConfig: function() {
                this.callParent(arguments);
				
				 this.addColumnValidator("Phone", () => {
					let email = Ext.isEmpty(this.get("Email")) ? null : this.get("Email");
            		let phone = Ext.isEmpty(this.get("Phone")) ? null : this.get("Phone");
					return NIKO_ValidatorsHelper.phoneValidator(email, phone);
				});
				
				 this.addColumnValidator("Email", () => {
					let email = Ext.isEmpty(this.get("Email")) ? null : this.get("Email");
            		let phone = Ext.isEmpty(this.get("Phone")) ? null : this.get("Phone");
					return NIKO_ValidatorsHelper.emailValidator(email, phone);
				});
            },
			/**
			 * @inheritdoc Terrasoft.DuplicatesSearchUtilitiesV2#getFilterValue
			 * @override
			 */
			getFilterValue: function(filter) {
				if (this.isDuplicateRuleByName(filter)) {
					return this.getContactNameForSearchDuplicates();
				}
				return this.callParent(arguments);
			},
			//#endregion Filters and Validations functions
			
			/* Получить ключ контакта. */
			getKeyFromContact: function() {
				let phone = this.get("Phone");
				let email = this.get("Email");
				
				return (!Ext.isEmpty(phone) && !Ext.isEmpty(email)) 
					? email 
					: !Ext.isEmpty(phone)
						? phone
						: !Ext.isEmpty(email)
							? email
							: null;
			},
			/**
			 * Кнопки в меню Actions.
			 * @overriden
			 */
			getActions: function() {
				var actionMenuItems = this.callParent(arguments);
				
				actionMenuItems.addItem(this.getButtonMenuItem({
					"Caption": {"bindTo": "Resources.Strings.GetInfoFromContact"},
					"Tag": "getInfoFromContact",
					"Enabled": {"bindTo": "GetInfoFromContactEnabled"}
				}));
				
				return actionMenuItems;
			},
			
			//#region Events
			/**
			 * Событие переключения IsActive.
			 * @overriden
			 */
			toggleGetInfoFromContact: function() {
				let currentVal = this.get("NIKO_IsActive");
				this.set("GetInfoFromContactEnabled", currentVal);
				this.sandbox.publish("GetInfoFromContactChanged", currentVal, ["SectionModuleV2_ContactSectionV2"]);
			},
			/**
			 * Событие нажатия на кнопку "Get Info".
			 * @overriden
			 */
			getInfoFromContact: function() {
				this.loadGetInfoModalBox();
			},
			/* Загрузка модального окна. */
			loadGetInfoModalBox: function() {
				var sandbox = this.sandbox;
				var config = {
					heightPixels: 250,
					widthPixels: 580
				};
				var moduleName = "NIKO_GetInfoModalBox";
				var moduleId = sandbox.id + "_" + moduleName;
				var renderTo = ModalBox.show(config, function() {
					sandbox.unloadModule(moduleId, renderTo);
				});
				sandbox.loadModule(moduleName, {
					id: moduleId,
					renderTo: renderTo
				});
			},
			/** Отправка данных текущего контакта. */
			sendValuesToModalBox: function() {
				var config = {
					FullName: this.get("Name"),
					Key: this.getKeyFromContact()
				};
				this.sandbox.publish("LoadFieldValues", config,
					[this.sandbox.id + "_" + "NIKO_GetInfoModalBox"]);
			},
			/*
			 * Событие нажатия на кнопку Get в модальном окне. 
			* @param {String} obj Данные из модального окна.
			 */
			onGetButtonClick: function(obj) {
				var config = {
					callback: undefined,
					args: obj,
				};
				this.getAnswerFromNIKO_ContactsSyncService(config);
			},
			//#endregion Events
			
			//#region Utils

			/**
			 * Запустить (Promise) сервис с параметрами.
			 * @param {String} name Название сервиса.
			 * @param {String} method Вызываемый метод.
			 * @param {Object} data Аргументы передаваемые в сервис.
			 * @returns 
			 */
			runService: function(name, method, data) {
				return new Promise((resolve, reject) => {
					const config = {
						serviceName: name,
						methodName: method,
						data: data,
						callback: resolve,
						scope: this
					}
					ServiceHelper.callService(config);
				});
			},
			
			//#endregion Utils
			
			/**
			 * Получить ответ от сервиса "NIKO_ContactsSyncService (ContactsSyncService)".
			 * @param {Function} callback Метод, который отрабатывает в случае успеха.
			 * @param {Object} args Аргументы используемые в data.
			 */
			getAnswerFromNIKO_ContactsSyncService: function(config) {
				const data = { 
					id: this.$Id, 
					key: config.args?.Key, 
					fullName: config.args?.FullName,
				};

				this.runService("NIKO_ContactsSyncService", "GetContactInfo", data).then(result => {
					var response = result?.GetContactInfoResult;
					this.reloadEntity();
					if (response?.error) {
						Terrasoft.showErrorMessage(response?.result);
					}
					else {
						Terrasoft.showInformation(response?.result);
					}
				});
			},
			
		},
		modules: /**SCHEMA_MODULES*/{}/**SCHEMA_MODULES*/,
		dataModels: /**SCHEMA_DATA_MODELS*/{}/**SCHEMA_DATA_MODELS*/,
		diff: /**SCHEMA_DIFF*/[
			{
				"operation": "merge",
				"name": "PhotoTimeZoneContainer",
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
				"name": "AccountName",
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
				"operation": "merge",
				"name": "AccountPhone",
				"values": {
					"layout": {
						"colSpan": 24,
						"rowSpan": 1,
						"column": 0,
						"row": 2
					},
					"enabled": true
				}
			},
			{
				"operation": "remove",
				"name": "AccountPhone",
				"properties": [
					"contentType"
				]
			},
			{
				"operation": "merge",
				"name": "AccountEmail",
				"values": {
					"layout": {
						"colSpan": 24,
						"rowSpan": 1,
						"column": 0,
						"row": 3
					}
				}
			},
			{
				"operation": "insert",
				"name": "NIKO_IsActive258cb96b-5e76-4bfd-ba48-2891b8b345b4",
				"values": {
					"layout": {
						"colSpan": 24,
						"rowSpan": 1,
						"column": 0,
						"row": 4,
						"layoutName": "ProfileContainer"
					},
					"bindTo": "NIKO_IsActive"
				},
				"parentName": "ProfileContainer",
				"propertyName": "items",
				"index": 4
			},
			{
				"operation": "merge",
				"name": "GeneralInfoTab",
				"values": {
					"order": 0
				}
			},
			{
				"operation": "insert",
				"name": "City08cf51b8-26eb-42aa-8ff9-7d45a9800798",
				"values": {
					"layout": {
						"colSpan": 12,
						"rowSpan": 1,
						"column": 0,
						"row": 0,
						"layoutName": "ContactGeneralInfoBlock"
					},
					"bindTo": "City"
				},
				"parentName": "ContactGeneralInfoBlock",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "NIKO_Home1430d0e8-4a6e-4801-ad34-e084809787a5",
				"values": {
					"layout": {
						"colSpan": 12,
						"rowSpan": 1,
						"column": 12,
						"row": 0,
						"layoutName": "ContactGeneralInfoBlock"
					},
					"bindTo": "NIKO_Home"
				},
				"parentName": "ContactGeneralInfoBlock",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "BirthDatefe4b98a0-fc4d-40a6-bfb4-9bdc35870292",
				"values": {
					"layout": {
						"colSpan": 12,
						"rowSpan": 1,
						"column": 0,
						"row": 1,
						"layoutName": "ContactGeneralInfoBlock"
					},
					"bindTo": "BirthDate"
				},
				"parentName": "ContactGeneralInfoBlock",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "merge",
				"name": "Gender",
				"values": {
					"layout": {
						"colSpan": 12,
						"rowSpan": 1,
						"column": 12,
						"row": 1
					}
				}
			},
			{
				"operation": "insert",
				"name": "NIKO_ContactAddress_Detail",
				"values": {
					"itemType": 2,
					"markerValue": "added-detail"
				},
				"parentName": "GeneralInfoTab",
				"propertyName": "items",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "NIKO_PaymentSchedule_Detail",
				"values": {
					"itemType": 2,
					"markerValue": "added-detail"
				},
				"parentName": "GeneralInfoTab",
				"propertyName": "items",
				"index": 2
			},
			{
				"operation": "remove",
				"name": "JobTitleProfile"
			},
			{
				"operation": "remove",
				"name": "AccountMobilePhone"
			},
			{
				"operation": "remove",
				"name": "Type"
			},
			{
				"operation": "remove",
				"name": "Owner"
			},
			{
				"operation": "remove",
				"name": "SalutationType"
			},
			{
				"operation": "remove",
				"name": "Age"
			},
			{
				"operation": "remove",
				"name": "Language"
			},
			{
				"operation": "remove",
				"name": "ContactCommunication"
			},
			{
				"operation": "remove",
				"name": "ContactAddress"
			},
			{
				"operation": "remove",
				"name": "ContactAnniversary"
			},
			{
				"operation": "remove",
				"name": "Relationships"
			},
			{
				"operation": "remove",
				"name": "JobTabContainer"
			},
			{
				"operation": "remove",
				"name": "JobInformationControlGroup"
			},
			{
				"operation": "remove",
				"name": "JobInformationBlock"
			},
			{
				"operation": "remove",
				"name": "Job"
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
				"name": "DecisionRole"
			},
			{
				"operation": "remove",
				"name": "ContactCareer"
			},
			{
				"operation": "remove",
				"name": "HistoryTab"
			},
			{
				"operation": "remove",
				"name": "Activities"
			},
			{
				"operation": "remove",
				"name": "EmailDetailV2"
			},
			{
				"operation": "remove",
				"name": "NotesAndFilesTab"
			},
			{
				"operation": "remove",
				"name": "Files"
			},
			{
				"operation": "remove",
				"name": "NotesControlGroup"
			},
			{
				"operation": "remove",
				"name": "Notes"
			}
		]/**SCHEMA_DIFF*/
	};
});
