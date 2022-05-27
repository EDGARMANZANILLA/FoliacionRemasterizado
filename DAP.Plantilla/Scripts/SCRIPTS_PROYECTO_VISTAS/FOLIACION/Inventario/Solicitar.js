﻿



// //El siguiente par es para el CRUD de los bancos agregados a la solicitud
function DibujarTablaAjax() {
    $("#divTablaCrud").append(

        "<table id='tablaAgregados'  class='table table-striped display table-bordered table-hover' cellspacing='0'  style='width:100%'>" +
        " <caption class='text-uppercase'>Detalle general de formas de pago inhabilitadas </caption>"
        + "<thead class='tabla'>" +

        "<tr>" +
        "<th>Id </th>" +
        "<th>IdBanco </th>" +
        "<th>Banco </th>" +
        "<th>Cuenta</th>" +
        "<th>Cantidad</th>" +
        "<th>Folio Inicial</th>" +
        "<th></th>" +

        "</tr>" +
        "</thead>" +
        "</table>"
    );
};

let listar;
function PintarConsultas(datos) {

    listar = $('#tablaAgregados').DataTable({
        "ordering": false,
        "info": false,
        "searching": false,
        "paging": true,
        "lengthMenu": [5, 10],
        "language":
        {
            "processing": "Procesando...",
            "lengthMenu": "Mostrar _MENU_ registros",
            "zeroRecords": "No se encontraron resultados",
            "emptyTable": "Ningún dato disponible en esta tabla",
            "infoEmpty": "Mostrando registros del 0 al 0 de un total de 0 registros",
            "infoFiltered": "(filtrado de un total de _MAX_ registros)",
            "search": "Buscar:",
            "info": "Mostrando de _START_ a _END_ de _TOTAL_ entradas",
            "paginate": {
                "first": "Primero",
                "last": "Último",
                "next": "Siguiente",
                "previous": "Anterior"
            }
        },
        "data": datos,
        "columns": [
            { "data": "Id" },
            { "data": "IdBanco" },
            { "data": "cadenaNombreBanco" },
            { "data": "cuentaBanco" },
            { "data": "cantidadFormas" },
            { "data": "fInicial" },
            { "defaultContent": "<a class='editar btn btn-success' data-toggle='modal' data-target='#EditarSolicitud'  > <i class='fas fa-edit'></i></a> <a class='eliminar btn btn-danger'> <i class='fas fa-trash-alt'></i></a>" }


        ]

    });
};

//  //El siguiente par es para el historico de las solicitudes

function DibujarTablaSolicitudesCreadasAjax() {
    $("#TablaHistorica").append(

        "<table id='tbHistoricoSolicitudesCreadas'  class='table table-striped display table-bordered table-hover' cellspacing='0'  style='width:100%'>" +
        " <caption class='text-uppercase'>Historico de solicitudes creadas </caption>"
        + "<thead class='tabla'>" +
        "<tr>" +
        "<th>Numero Memorandum</th>" +
        "<th>Fecha Solicitud</th>" +
        "<th>  </th>" +
        "</tr>" +
        "</thead>" +
        "<tfoot>" +
        "<tr>" +
        "<th class='Filtro'>No. Memo</th>" +
        "<th class='Filtro'>Fecha</th>" +
        "<th></th>" +
        "</tr>" +
        "</tfoot>" +
        "</table>"
    );
};


let historioSolicitudes;
function PintarConsultaTablaSolicitud(datos) {

    historioSolicitudes = $('#tbHistoricoSolicitudesCreadas').DataTable({
        "ordering": true,
        "info": false,
        "searching": true,
        "paging": true,
        "lengthMenu": [10, 15],
        "language":
        {
            "processing": "Procesando...",
            "lengthMenu": "Mostrar _MENU_ registros",
            "zeroRecords": "No se encontraron resultados",
            "emptyTable": "Ningún dato disponible en esta tabla",
            "infoEmpty": "Mostrando registros del 0 al 0 de un total de 0 registros",
            "infoFiltered": "(filtrado de un total de _MAX_ registros)",
            "search": "Buscar:",
            "info": "Mostrando de _START_ a _END_ de _TOTAL_ entradas",
            "paginate": {
                "first": "Primero",
                "last": "Último",
                "next": "Siguiente",
                "previous": "Anterior"
            }
        },
        "order": [[0, 'desc']],
        "data": datos,
        "columns": [
            { "data": "NumeroMemo" },
            { "data": "FechaSolicitud" },
            {

                "render": function (data, type, row) {
                    let da = row.NumeroMemo;
                    return '<a  class="detalles btn btn-success" data-toggle="modal" data-target="#DetalleSolicitud">Ver detalles </a> <a class="descargar btn btn-primary" href="/Inventario/GenerarReporteSolicitud?NumMemorandum=' + da + '"> <i class="fas fa-cloud-download-alt"></i></a>   <a class="eliminarMemorandum btn btn-danger"> <i class="fas fa-trash-alt"></i></a>';
                }


            }

        ]

    });
};


//funciones para los detalles de las solicitudes filtrados por cadaMemorandum
function DibujarTablaDetalleSolicitudAjax() {
    $("#TablaDetalleSolicitud").append(

        "<table id='tbDetalleSolicitud'  class='table table-striped display table-bordered table-hover' cellspacing='0'  style='width:100%'>" +
        " <caption class='text-uppercase'>Detalle de la Solicitud </caption>"
        + "<thead class='tabla'>" +
        "<tr>" +
        "<th>Numero Memorandum</th>" +
        "<th>Banco</th>" +
        "<th>Cuenta</th>" +
        "<th>Cantidad  </th>" +
        "<th>Folio Inicial  </th>" +
        "</tr>" +
        "</thead>" +

        "</table>"
    );
};

function PintarConsultaDetalleSolicitud(datos) {

    $('#tbDetalleSolicitud').DataTable({
        "ordering": false,
        "info": false,
        "searching": false,
        "paging": true,
        "lengthMenu": [5, 10],
        "language":
        {
            "processing": "Procesando...",
            "lengthMenu": "Mostrar _MENU_ registros",
            "zeroRecords": "No se encontraron resultados",
            "emptyTable": "Ningún dato disponible en esta tabla",
            "infoEmpty": "Mostrando registros del 0 al 0 de un total de 0 registros",
            "infoFiltered": "(filtrado de un total de _MAX_ registros)",
            "search": "Buscar:",
            "info": "Mostrando de _START_ a _END_ de _TOTAL_ entradas",
            "paginate": {
                "first": "Primero",
                "last": "Último",
                "next": "Siguiente",
                "previous": "Anterior"
            }
        },
        "order": [[1, 'asc']],
        "data": datos,
        "columns": [
            { "data": "NumeroMemo" },
            { "data": "NombreBanco" },
            { "data": "NumeroCuenta" },
            { "data": "Cantidad" },
            { "data": "FolioInicial" },
        ]

    });
};



///***********************       FUNCIONES PARA CREAR UNA SOLICITUD DE CUENTAS BANCARIAS  *******************************/
listaBancosSolicitados = new Array();
function CrearNuevaSolicitudFormasPago() {
    $("#TablaHistorica").empty();
    ////pintar de nuevo la tabla +

    DibujarTablaAjax();

    //PintarConsultas(listaBancosSolicitados);

    let nuevaSolicitud = document.getElementById('NuevaSolicitud').classList.remove('btn-outline-primary');
    nuevaSolicitud = document.getElementById('NuevaSolicitud').classList.add('btn-primary');

    let inhabilitarContenedor = document.getElementById('Historico').classList.remove('btn-primary');
    inhabilitarContenedor = document.getElementById('Historico').classList.add('btn-outline-primary');

    //let InhabilitarFolios = document.getElementById('InhabilitarFolios').classList.remove('btn-primary');
    //InhabilitarFolios = document.getElementById('InhabilitarFolios').classList.add('btn-outline-primary');


    //document.getElementById("IFolios").style.display = "none";
    document.getElementById("AgregarBancosASolicitud").style.display = "block";
    document.getElementById("CrearSolicitud").style.display = "block";

}



let bancoSeleccionado = null;
let cuentaBanco = null;
let cantidadFormas = null;
let fInicial = null;
let Id = 0;
let texto;

function Anexar_Solicitud() {


    cantidadFormas = document.getElementById('CantidadFormas').value;
    fInicial = document.getElementById("FInicial").value;
    bancoSeleccionado = document.getElementById("SeleccionBancoSolicitud");

    texto = bancoSeleccionado.options[bancoSeleccionado.selectedIndex].innerText;

    let cadenaSeparada = texto.split("||");


    let cadenaNombreBanco = cadenaSeparada[0];
    cuentaBanco = cadenaSeparada[1];



    if (bancoSeleccionado.value != 0) {
        if (cantidadFormas != null && cantidadFormas != "") {
            Id += 1;
            let IdBanco = bancoSeleccionado.value;

            listaBancosSolicitados.push({ Id, IdBanco, cadenaNombreBanco, cuentaBanco, cantidadFormas, fInicial });

            //   console.log(Id, cadenaSeparada[0], cadenaSeparada[1], cantidadFormas, fInicial);


            $("#divTablaCrud").empty();
            DibujarTablaAjax();
            PintarConsultas(listaBancosSolicitados);



        } else {

            MensajeWarningSweet('Ingrese una cantidad de formas');
        }

    } else {

        MensajeWarningSweet('Seleccione un banco');
    }

}




//Click para eliminar el elemento seleccionado de la tabla
$(document).on("click", ".eliminar", function () {



    //console.log("vieja" + listaBancosSolicitados);

    let datoAEliminar = listar.row($(this).parents("tr")).data();
    var i = listaBancosSolicitados.indexOf(datoAEliminar);
    listaBancosSolicitados.splice(i, 1);



    // console.log(listaBancosSolicitados);


    let eliminado = listar.row($(this).parents("tr")).remove().draw();
});



let datoAEditar;
//Click para editar el elemento seleccionado de la tabla
$(document).on("click", ".editar", function () {

    datoAEditar = listar.row($(this).parents("tr")).data();
    // console.log(datoAEditar);
    var i = listaBancosSolicitados.indexOf(datoAEditar);

    // console.log(listaBancosSolicitados[i].nombreBanco);

    let nBanco = listaBancosSolicitados[i].nombreBanco;

    document.getElementById("IdEdicion").innerHTML = listaBancosSolicitados[i].Id;
    $("#EditarSeleccionNombreBanco").val(listaBancosSolicitados[i].IdBanco);
    //$('#EditarSeleccionNombreBanco').change();
    document.getElementById("EditarCantidadFormas").value = listaBancosSolicitados[i].cantidadFormas;
    document.getElementById("EditarFInicial").value = listaBancosSolicitados[i].fInicial;









    //obtner datos dela edicion del banco
    const GuardarEdicion = document.getElementById("GuardarEdicion");
    GuardarEdicion.addEventListener("click",
        function () {
            //se muestra el mensaje cargando para bloquear la pantalla
            MensajeCargando();
            /// Editar(datoAEditar.Id);


            let indice = listaBancosSolicitados.findIndex(y => y.Id == datoAEditar.Id);
            //  console.log("asdf");
            //  console.log(indice);




            let bancoEditado = document.getElementById("EditarSeleccionNombreBanco");
            let htmlObtendidoDeSeleccion = bancoEditado.options[bancoEditado.selectedIndex].innerText;
            let cadenaSeparadaSplit = htmlObtendidoDeSeleccion.split("||");


            let cadenaNombreBancoEditado = cadenaSeparadaSplit[0];
            let cuentaBancoEditado = cadenaSeparadaSplit[1];




            //Obtiene los datos modificados del modal
            let cantidadFormasEdicion = document.getElementById('EditarCantidadFormas').value;
            let folioInicialEdicion = document.getElementById('EditarFInicial').value;

            //se actualizan los varlores dentro del arreglo

            listaBancosSolicitados[indice].IdBanco = bancoEditado.value;
            listaBancosSolicitados[indice].cadenaNombreBanco = cadenaNombreBancoEditado;
            listaBancosSolicitados[indice].cuentaBanco = cuentaBancoEditado;
            listaBancosSolicitados[indice].cantidadFormas = cantidadFormasEdicion;
            listaBancosSolicitados[indice].fInicial = folioInicialEdicion;


            //se vacia la tabla
            $("#divTablaCrud").empty();

            DibujarTablaAjax();

            PintarConsultas(listaBancosSolicitados);

            OcultarMensajeCargando();

            $('#EditarSolicitud').modal('hide');

            //  console.log(listaBancosSolicitados);
        }
    );


});







function CrearSolicitudFormasPago() {

    if (listaBancosSolicitados.length > 0) {
        MensajeCargando();
        $.ajax({
            url: 'CrearNuevaSolicitud',
            data: JSON.stringify(listaBancosSolicitados),
            type: "POST",
            contentType: "application/json; charset=utf-8",
            success: function (msg) {
                OcultarMensajeCargando();
                //console.log("msg", msg);
                if (msg == true) {
                    $('#Descargar').modal('show');

                } else {
                    Swal.fire({
                        backdrop: true,
                        allowEnterKey: false,
                        allowOutsideClick: false,
                        icon: 'error',
                        title: 'No se pudo crear la solicitud; Intente de nuevo!',

                    })
                }



            },
            error: function (msg) {

                OcultarMensajeCargando();
                Swal.fire({
                    backdrop: true,
                    allowEnterKey: false,
                    allowOutsideClick: false,
                    icon: 'error',
                    title: 'Ocurrio un problema intente de nuevo',

                })
            }
        });



    } else {

        Swal.fire({
            backdrop: true,
            allowEnterKey: false,
            allowOutsideClick: false,
            icon: 'warning',
            title: 'Agregue un banco a la solicitud',

        })
    }







}










/*******************************************************************************************************************************************/
/*******************************************************************************************************************************************/
/**************************         Metodos para mostrar el historico de las solicitudes creadas         ***********************************/


function VerHistoricoSolicitudes() {

    $("#divTablaCrud").empty();
    let nuevaSolicitud = document.getElementById('Historico').classList.remove('btn-outline-primary');
    nuevaSolicitud = document.getElementById('Historico').classList.add('btn-primary');

    let inhabilitarContenedor = document.getElementById('NuevaSolicitud').classList.remove('btn-primary');
    inhabilitarContenedor = document.getElementById('NuevaSolicitud').classList.add('btn-outline-primary');

    document.getElementById("AgregarBancosASolicitud").style.display = "none";
    document.getElementById("CrearSolicitud").style.display = "none";


    MensajeCargando();
    $.ajax({
        url: 'ObtenerHistoricoSolicitudes',
        data: JSON.stringify(listaBancosSolicitados),
        type: "POST",
        contentType: "application/json; charset=utf-8",
        success: function (resultado) {
            OcultarMensajeCargando();

            $("#TablaHistorica").empty();

            DibujarTablaSolicitudesCreadasAjax();
            PintarConsultaTablaSolicitud(resultado);


        },
        error: function (msg) {

            OcultarMensajeCargando();
         

            MensajeErrorSweet('', 'Ocurrio un problema intente de nuevo')
        }
    });

}







$(document).on("click", ".detalles", function () {

    let datosFilaSeleccionada = historioSolicitudes.row($(this).parents("tr")).data();

    console.log(datosFilaSeleccionada);

    MensajeCargando();


    let EnviarNumeroMemo = "{'NumeroMemo':'" + datosFilaSeleccionada.NumeroMemo + "'}";
    $.ajax({
        url: 'ObtenerSolicitudPorMemo',
        data: EnviarNumeroMemo,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        success: function (resultado) {
            OcultarMensajeCargando();
            console.log(resultado);


            $("#TablaDetalleSolicitud").empty();

            DibujarTablaDetalleSolicitudAjax();

            ////cambia
            PintarConsultaDetalleSolicitud(resultado);



        },
        error: function (msg) {

            OcultarMensajeCargando();
            Swal.fire({
                backdrop: true,
                allowEnterKey: false,
                allowOutsideClick: false,
                icon: 'error',
                title: 'Ocurrio un problema intente de nuevo',

            })
        }
    });





});










$(document).on("click", ".eliminarMemorandum", function () {

    MensajeCargando();

    //console.log("vieja" + listaBancosSolicitados);

    let filaHistoricoEliminar = historioSolicitudes.row($(this).parents("tr")).data();

    historioSolicitudes.row($(this).parents("tr")).remove().draw();

    console.log(filaHistoricoEliminar.NumeroMemo);


    let NumeroMemoEliminar = "{'NumeroMemorandum':'" + filaHistoricoEliminar.NumeroMemo + "'}";


    $.ajax({
        url: 'RemoverSolicitudCreada',
        data: NumeroMemoEliminar,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        success: function (resultado) {
            OcultarMensajeCargando();


            MensajeCorrectoSweet("Memorandum numero  " + filaHistoricoEliminar.NumeroMemo + " eliminado");

        },
        error: function (msg) {

            OcultarMensajeCargando();

            MensajeErrorSweet('', 'Ocurrio un problema intente de nuevo')
        }
    });





});






