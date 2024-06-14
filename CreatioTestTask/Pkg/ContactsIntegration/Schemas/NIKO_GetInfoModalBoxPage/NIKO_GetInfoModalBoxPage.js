define("NIKO_GetInfoModalBoxPage", ["ModalBox"], function(ModalBox) {
	return {
		attributes: {
			/* ФИО */
			"FullName": {
				dataValueType: Terrasoft.DataValueType.TEXT,
				type: Terrasoft.ViewModelColumnType.VIRTUAL_COLUMN
			},
			/* Ключ */
			"Key": {
				dataValueType: Terrasoft.DataValueType.TEXT,
				type: Terrasoft.ViewModelColumnType.VIRTUAL_COLUMN
			},
			/* Аттрибут, который показывает доступность кнопки "GetButtonEnabled" 
			 * зависит от заполненных полей "Key" и "FullName". 
			 */
			"GetButtonEnabled": {
				"dataValueType": Terrasoft.DataValueType.BOOLEAN,
				"type": Terrasoft.ViewModelColumnType.VIRTUAL_COLUMN,
				"value": false
			},
		},
		messages: {
			/* Данные из модального окна. */
			"DataFromModal": {
				mode: Terrasoft.MessageMode.PTP,
				direction: Terrasoft.MessageDirectionType.PUBLISH
			},
			/* Событие страница загружена. */
			"ModalPageLoaded": {
				mode: Terrasoft.MessageMode.PTP,
				direction: Terrasoft.MessageDirectionType.PUBLISH
			},
			/* Загрузка значений в модальное окно. */
			"LoadFieldValues": {
				mode: Terrasoft.MessageMode.PTP,
				direction: Terrasoft.MessageDirectionType.SUBSCRIBE
			}
		},
		methods: {
			init: function(callback, scope) {
				this.callParent(arguments);
				this.sandbox.subscribe("LoadFieldValues", this.loadFieldValues, this, [this.sandbox.id]);
			},
			onRender: function() {
				this.sandbox.publish("ModalPageLoaded", null, [this.sandbox.id]);
			},
			onSaveButtonClick: function() {
				var config = {
					FullName: this.get("FullName"),
					Key: this.get("Key")
				};
				this.sandbox.publish("DataFromModal", config, [this.sandbox.id]);
				ModalBox.close();
			},
			onCloseButtonClick: function() {
				ModalBox.close();
			},
			/* Установить поля полученные со страницы. */
			loadFieldValues: function(obj) {
				this.set("FullName", obj?.FullName);
				this.set("Key", obj?.Key);
				this.$GetButtonEnabled = !Ext.isEmpty(obj?.FullName) && !Ext.isEmpty(obj?.Key);
			},
		},
		diff: [
			{
				"operation": "insert",
				"name": "MyContainer",
				"propertyName": "items",
				"values": {
					"itemType": Terrasoft.ViewItemType.CONTAINER,
					"items": []
				}
			},
			{
				"operation": "insert",
				"parentName": "MyContainer",
				"propertyName": "items",
				"name": "MyGridContainer",
				"values": {
					"itemType": Terrasoft.ViewItemType.GRID_LAYOUT,
					"items": []
				}
			},
			{
				"operation": "insert",
				"parentName": "MyGridContainer",
				"propertyName": "items",
				"name": "FullName",
				"values": {
					"enabled": false,
					"bindTo": "FullName",
					"caption": "Full Name",
					"layout": {"column": 0, "row": 0, "colSpan": 20}
				}
			},		
			{
				"operation": "insert",
				"parentName": "MyGridContainer",
				"propertyName": "items",
				"name": "Key",
				"values": {
					"enabled": false,
					"bindTo": "Key",
					"caption": "Key",
					"layout": {"column": 0, "row": 2, "colSpan": 20}
				}
			},	
			{
				"operation": "insert",
				"parentName": "MyGridContainer",
				"name": "SaveButton",
				"propertyName": "items",
				"values": {
					"itemType": Terrasoft.ViewItemType.BUTTON,
					"style": Terrasoft.controls.ButtonEnums.style.GREEN,
					"click": {bindTo: "onSaveButtonClick"},
					"enabled": {bindTo: "GetButtonEnabled"},
					"markerValue": "SaveButton",
					"caption": "Get",
					"layout": { "column": 8, "row": 4, "colSpan": 3 }
				}
			},
			{
				"operation": "insert",
				"parentName": "MyGridContainer",
				"name": "CloseButton",
				"propertyName": "items",
				"values": {
					"itemType": Terrasoft.ViewItemType.BUTTON,
					"style": Terrasoft.controls.ButtonEnums.style.GREY,
					"click": {bindTo: "onCloseButtonClick"},
					"markerValue": "CloseButton",
					"caption": "Cancel",
					"layout": { "column": 13, "row": 4, "colSpan": 3 }
				}
			},			
		]
	};
});