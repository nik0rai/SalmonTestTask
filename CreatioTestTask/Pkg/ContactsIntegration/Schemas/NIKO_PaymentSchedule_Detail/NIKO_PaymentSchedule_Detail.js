define("NIKO_PaymentSchedule_Detail", [], function() {
	return {
		entitySchemaName: "NIKO_PaymentSchedule",
		details: /**SCHEMA_DETAILS*/{}/**SCHEMA_DETAILS*/,
		diff: /**SCHEMA_DIFF*/[]/**SCHEMA_DIFF*/,
		methods: {
			/* Сделать фильтрацию по NIKO_IsActive. */
			addGridDataColumns: function(esq) {
				this.callParent(arguments);

				var date = esq.addColumn("NIKO_PaymentDateTime", "NIKO_PaymentDateTime");
				date.orderPosition = 0;
				date.orderDirection = Terrasoft.OrderDirection.DESC;
			},
		}
	};
});
