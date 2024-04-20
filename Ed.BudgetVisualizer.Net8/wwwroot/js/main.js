window.RenderDiagrams = (diagrams) => {
    am4core.useTheme(am4themes_material);
    let container = document.getElementById("diagram-container");
    container.innerHTML = "";

    for (var d = 0; d < diagrams.length; d++) {
        let diagram = diagrams[d];

        let chartDiv = document.createElement("div");
        chartDiv.setAttribute("id", diagram.title);
        chartDiv.setAttribute("class", "diagram mb-5");
        container.appendChild(chartDiv);

        let chart = am4core.create(diagram.title, am4charts.SankeyDiagram);
        chart.data = diagram.links;

        chart.dataFields.fromName = "from";
        chart.dataFields.toName = "to";
        chart.dataFields.value = "value";

        chart.paddingRight = 150;
        chart.paddingBottom = 20;

        // Add a title
        chart.titles.template.fontSize = 20;
        chart.titles.create().text = diagram.title;

        // Set a hover text
        let hoverState = chart.links.template.states.create("hover");
        hoverState.properties.fillOpacity = 0.6;

        // Set a link label
        //let labelBullet = chart.links.template.bullets.push(new am4charts.LabelBullet());
        //labelBullet.label.propertyFields.text = "value";
    }

    return true;
};