define("ContactSectionV2", [], function() {
	return {
		entitySchemaName: "Contact",
		attributes: {
			/* Аттрибут, который показывает доступность кнопки "GetInfoFromContact" 
			 * на странице ContactPageV2 зависит от "NIKO_IsActive". 
			 */
			"GetInfoFromContactEnabled": {
				"dataValueType": Terrasoft.DataValueType.BOOLEAN,
				"type": Terrasoft.ViewModelColumnType.VIRTUAL_COLUMN,
				"value": false
			},
		},
		details: /**SCHEMA_DETAILS*/{}/**SCHEMA_DETAILS*/,
		diff: /**SCHEMA_DIFF*/[]/**SCHEMA_DIFF*/,
		messages: {
			/* Сообщение о том, что IsActive было переключено. */
			"GetInfoFromContactChanged": {
				mode: Terrasoft.MessageMode.PTP,
				direction: Terrasoft.MessageDirectionType.SUBSCRIBE
			},
		},
		methods: {
			/** 
			 * Подписка на сообщения.
			 * @overriden
			 */
			subscribeSandboxEvents: function() {
				this.callParent(arguments);		
				this.sandbox.subscribe("GetInfoFromContactChanged", this.toggleGetInfoFromContact, this, [this.sandbox.id]);
			},			
			/** 
			 * Событие изменения статуса активности. 
			 */
			toggleGetInfoFromContact: function(response) {
				this.set("GetInfoFromContactEnabled", response);
			},
		}
	};
});