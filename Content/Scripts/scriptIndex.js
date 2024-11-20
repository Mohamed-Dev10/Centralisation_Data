// Ensure the DOM is fully loaded before executing JavaScript code
document.addEventListener('DOMContentLoaded', function() {
    // Your existing code for sidebar toggle, search form behavior, and switch mode

    // Sample data for the graphs
    var data1 = {
        labels: ['Complétés', 'En attente', 'Processus'],
        datasets: [{
            data: [5, 3, 2],
            backgroundColor: ['#36A2EB', '#FFCE56', '#FF6384']
        }]
    };

    var data2 = {
        labels: ['Administration', 'Finance Et Assurance', 'Transport Et Messagerie','Industrie'],
        datasets: [{
            data: [10, 20, 30,40],
            backgroundColor: ['#36A2EB', '#FFCE56', '#FF6384','#808080']
        }]
    };

    // Get the canvas elements
    var ctx1 = document.getElementById('graph1').getContext('2d');
    var ctx2 = document.getElementById('graph2').getContext('2d');

    // Create the first graph (circular graph)
    var chart1 = new Chart(ctx1, {
        type: 'doughnut',
        data: data1
    });

    // Create the second graph (different type of graph)
    var chart2 = new Chart(ctx2, {
        type: 'bar', // Change the type of graph as needed
        data: data2
    });
});


document.addEventListener('DOMContentLoaded', function() {
    var initialData = [65, 59, 80, 81, 56, 55, 40,35,30,85,90,95];
    var chartData = {
        labels: ['Janvier','Février','Mars','Avril','Mai','Juin','Juillet','Août','Septembre','Octobre','Novembre','Décembre'],
        datasets: [{
            label: 'Données',
            backgroundColor: 'rgba(54, 162, 235, 0.2)',
            borderColor: 'rgba(54, 162, 235, 1)',
            borderWidth: 1,
            data: initialData,
        }]
    };

    var chartCanvas = document.getElementById('chartCanvas').getContext('2d');
    var chart = new Chart(chartCanvas, {
        type: 'line',
        data: chartData,
        options: {
            responsive: true,
            maintainAspectRatio: false,
            animation: {
                easing: 'linear',
            },
        },
    });

    function updateChart() {
        var newData = initialData.map(function(value) {
            return Math.random() * 100;
        });
        chart.data.datasets[0].data = newData;
        chart.update();
    }

    setInterval(updateChart, 2000);
});

