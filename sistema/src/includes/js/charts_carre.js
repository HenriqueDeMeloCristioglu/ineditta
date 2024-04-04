//Gráfico Pie - Tela perfil 1,2 e 6,7,8
// var ctx = document.getElementById('myChart');
// if(ctx) {
//     ctx.getContext('2d');
//     var myChart = new Chart(ctx, {
//         type: 'bar',
//         data: {
//             labels: ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun', 'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez'],
//             datasets: [{
//                 label: 'Negociações em Aberto',
//                 data: [12, 19, 3, 5, 2, 3, 12, 19, 3, 5, 2, 3],
//                 backgroundColor: [
//                     'rgba(54, 162, 235, 0.2)'
                
//                 ],
//                 borderColor: [
//                     'rgba(54, 162, 235, 1)'
//                 ],
//                 borderWidth: 1
//             }]
//         },
//         options: {
//             scales: {
//                 y: {
//                     beginAtZero: true
//                 }
//             }
//         }
//     });
// }

//atualiza pls
//Gráfico Linha - Tela perfil 1,2
// var ctxLine = document.getElementById('lineChart');

// if (ctxLine) {
//     ctxLine.getContext('2d');

//     var myChart = new Chart(ctxLine, {
//     type: 'line',
//     data: {
//         labels: ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun', 'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez'], //[4.22,4.94,5.59,6.90,7.22,7.85,8.42,8.78,9.08,8.96,8.16,8.60],
//         datasets: [{
//             label: '2022',
//             data: [6.22,6.94,7.59,8.90,9.22,9.85,10.42,10.78,11.08,10.96,10.16,10.60],
            
//             // backgroundColor: [
//             //     'rgba(54, 162, 235, 0.2)'
               
//             // ],
//             borderColor: [
//                 'rgba(54, 162, 235, 1)'
//             ],
//             borderWidth: 1
//         },
//         {
//             label: '2021',
//             data: [4.22,4.94,5.59,6.90,7.22,7.85,8.42,8.78,10.08,8.96,8.16,8.60],
            
//             // backgroundColor: [
//             //     'rgba(54, 162, 235, 0.2)'
               
//             // ],
//             borderColor: [
//                 'rgba(248,155,28, 1)'
//             ],
//             borderWidth: 1
//         }]
//     },
//     options: {
//         scales: {
//             y: {
//                 beginAtZero: true,
//                 title: {
//                     display: true,
//                     text: "Percentual (%)",
//                     color: "000"
//                 }
//             }
//         }
//     }
// });
// }

//Gráfico Pie - Tela perfil 6,7,8
var ctxPie = document.getElementById('pieChart');

if (ctxPie) {
    ctxPie.getContext('2d');

    var pieChart = new Chart(ctxPie, {
        type: 'pie',
        data: {
            labels: ['Permitido', 'Proíbido', 'Proíbido - com Possibilidade de Acordo'], //[4.22,4.94,5.59,6.90,7.22,7.85,8.42,8.78,9.08,8.96,8.16,8.60],
            datasets: [
                {
                label: 'Dataset 1',
                data: [5,12,20],
                backgroundColor: [
                    'rgba(255, 159, 64, 0.7)',
                    'rgba(54, 162, 235, 0.7)',
                    'rgba(255, 99, 13, 0.7)'
                ]
                }
            ]
        },
        options: {
            plugins: {
                title: {
                    display: true,
                    text: 'Feriado 15/11 - Proclamação da República'
                }
            }
        }
    });
}

//Gráfico Bar - Modulo sindicatos
var ctxSind = document.getElementById('sindChart');
if(ctxSind) {
    ctxSind.getContext('2d');
    var chartSind = new Chart(ctxSind, {
        type: 'bar',
        data: {
            labels: ['AL', 'BA', 'CE', 'DF', 'ES', 'GO', 'MG', 'PE', 'PR', 'RJ', 'RN', 'RS', 'SC', 'SE', 'SP'],
            datasets: [{
                label: 'Total',
                data: [2, 3, 2, 2, 2, 3, 10, 5, 11, 7, 2, 8, 6, 3, 23],
                backgroundColor: [
                    'rgba(54, 162, 235, 0.2)'
                
                ],
                borderColor: [
                    'rgba(54, 162, 235, 1)'
                ],
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true
                }
            },
            plugins: {
                title: {
                    display: true,
                    text: "Contagem de Localidade-Unidade"
                }
            }
        }
    });
}

//Gráfico Pie - Tela modulo sindicatos
var ctxPieQtd = document.getElementById('qtdChart');

if (ctxPieQtd) {
    ctxPieQtd.getContext('2d');

    var pieChart = new Chart(ctxPieQtd, {
        type: 'pie',
        data: {
            labels: [
                        'CENTRAL DOS TRABALHADORES E TRABALHADORAS DO BRASIL', 'CENTRAL UNICA DOS TRABALHADORES - CUT', 'FORÇA SINDICAL', 'NOVA CENTRAL SINDICAL DE TRABALHADORES - NCST',
                        'CENTRAL SINDICAL E POPULAR CONLUTAS', 'CENTRAL GERAL DOS TRABALHADORES DO BRASIL - CGTB', 'Não há declaração de filiação', 'UNIÃO GERAL DOS TRABALHADORES - UGT'
                    ],
            datasets: [
                {
                label: 'Dataset 1',
                data: [7,15,19,15, 1, 1, 3, 28],
                backgroundColor: [
                    'rgba(255, 159, 64, 0.7)',
                    'rgba(54, 162, 235, 0.7)',
                    'rgba(255, 99, 13, 0.7)',
                    'rgba(255,99,132, 0.7)',
                    'rgba(201,203,207, 0.7)',
                    'rgba(153,102,255, 0.7)',
                    'rgba(80,102,180, 0.7)',
                    'rgba(255,205,86, 0.7)'
                ]
                }
            ]
        },
        options: {
            plugins: {
                // title: {
                //     display: true,
                //     text: 'Feriado 15/11 - Proclamação da República'
                // }
                legend: {
                    position: 'right'
                }
            }
        }
    });
}