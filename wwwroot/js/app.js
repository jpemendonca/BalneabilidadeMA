var app = new Vue({
    el: "#app",
    data: {
        Coordinates: [],
        Success: false,
        LastUpdate: ""
    },
    methods: {
        GetCoordinates() {
            var self = this;

            $.ajax({
                url: '/api/Buscar',
                type: 'GET',
                dataType: 'json',
                success: function (data) {
                    self.Success = true;
                    self.Coordinates = data
                    self.LastUpdate = data[0].Data
                    self.CreateMap();
                },
                error: function (xhr, status, error) {
                    self.CreateMap();
                    console.error('Erro na requisição:', status, error);
                }
            });
        },
        CreateMap() {
            var self = this;

            var map = L.map('map').setView([-2.495227, -44.229086], 13);


            L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
            }).addTo(map);

            map.locate({ setView: true, maxZoom: 16 });

            map.on('locationfound', function (e) {
                var radius = Math.min(e.accuracy / 2, 300);

                L.marker(e.latlng).addTo(map)
                    .bindPopup("Você está aqui").openPopup();

                L.circle(e.latlng, radius).addTo(map);
            });

            if (self.Success) {
                var redFlagIcon = L.icon({
                    iconUrl: '/images/red-flag.png',
                    iconSize: [19, 47.5],
                    iconAnchor: [9.5, 47.5],
                    popupAnchor: [-3, -76]
                });

                var greenFlagIcon = L.icon({
                    iconUrl: '/images/green-flag.png',
                    iconSize: [19, 47.5],
                    iconAnchor: [9.5, 47.5],
                    popupAnchor: [-3, -76]
                });

                self.Coordinates.forEach(coord => {
                    var lat = coord.latitude;
                    var lon = coord.longitude;


                    var icon = coord.estaProprioParaBanho ? greenFlagIcon : redFlagIcon;

                    var popupContent = `
            <div>
                <h5>Situação: ${coord.condicao}</h5>
                <p><strong>Localização:</strong> ${coord.localizacao || 'N/A'}</p>
                <p><strong>Referência:</strong> ${coord.referencia || 'N/A'}</p>
                <p><strong>Ponto:</strong> ${coord.ponto || 'N/A'}</p>
            </div>`;

                    L.marker([lat, lon], { icon: icon }).addTo(map)
                        .bindPopup(popupContent);
                });
            }
        }
    },
    created: function () {
        var self = this;

        self.GetCoordinates();
    }
});

function IsValidLatLon(num) {
    if (!num) {
        return false;
    }

    if (!isFinite(num)) {
        return false;
    }

    return true;
}