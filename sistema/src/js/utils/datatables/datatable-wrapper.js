import 'datatables.net-bs5/css/dataTables.bootstrap5.css';
import 'datatables.net-responsive-bs5/css/responsive.bootstrap5.css';
import $ from 'jquery';
import 'datatables.net-bs5';
import 'datatables.net-responsive-bs5';
import ptBrTranslation from '../../translate/pt-BR.json';
import moment from 'moment';
import 'moment/locale/pt-br';

class DataTableWrapper {
    constructor(selector, options) {
        this.selector = selector;

        this.defaultOptions = {
            processing: true,
            serverSide: true,
            ordering: true,
            searching: true,
            lengthChange: true,
            paging: true,
            responsive: true,
            columnDefs: [
                {
                    targets: "_all",
                    defaultContent: ""
                }
            ],
            language: {
                ...ptBrTranslation
            },
            searchDelay: 3000,
            lengthMenu: [ 10, 25, 50, 75, 100, 500, 1000 ]
        };

        this.options = { ...this.defaultOptions, ...options };

        this.dataTable = null;
        this.data = []
    }

    async initialize() {
        const { ajax, ...otherOptions } = this.options;

        const initializedOptions = {
            ...otherOptions,
            ajax: async (data, callback) => {
                const requestData = {
                    PageNumber: data.start / data.length,
                    PageSize: data.length,
                    SortColumn: data.columns[data.order[0].column].data,
                    SortOrder: data.order[0].dir,
                    Filter: data.search?.value
                };

                try {
                    const response = await ajax(requestData);

                    this.data = response.value.items

                    callback({
                        recordsTotal: response.value.totalCount,
                        recordsFiltered: response.value.totalCount,
                        data: response.value.items,
                    });
                } catch (error) {
                    console.error('Error fetching data:', error);
                }
            },
        };

        this.dataTable = $(this.selector).DataTable(initializedOptions);
    }

    async reload() {
        this.dataTable.ajax.reload();
        await Promise.resolve();
    }

    hideColumn(column) {
        this.dataTable.column(column).visible(false);
    }

    clear() {
        this.dataTable.clear().draw();
    }

    getValue() {
        return this.data
    }

    static formatDate(date) {
        if (!date || date === '0001-01-01') {
            return '';
        }

        return moment(date).format('DD/MM/YYYY');
    }

    static formatDateTime(date) {
        return moment(date).format('DD/MM/YYYY HH:mm');
    }

    disableColumn(index, isVisible) {
        this.dataTable.column(index).visible(isVisible);
    }

    getRows() {
        return this.dataTable.rows({ filter: 'applied' });
    }

    destroy() {
        return this.dataTable.destroy(true);
    }
}

export default DataTableWrapper;