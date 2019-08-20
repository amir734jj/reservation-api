angular.module('tourApp', ['ui.toggle', 'ngTagsInput'])
    .controller('eventInfoCtrl', ['$scope', '$http', '$window', function ($scope, $http, $window) {
        
        $scope.mapStudent = function ($event) {
            $event.preventDefault();
            
            var eventId = $scope.eventId;
            var studentId = $scope.studentId;
            
            if (eventId && studentId) {
                $http.post("/api/event/map/" + eventId + "/" + studentId).then(function () {
                    $scope.fetchInfo();
                });
            }
        };

        $scope.unMapStudent = function ($event, studentId) {
            $event.preventDefault();
            
            var eventId = $scope.eventId;
            
            if (eventId && studentId) {
                $http.post("/api/event/unmap/" + eventId + "/" + studentId).then(function () {
                    $scope.fetchInfo();
                });
            }
        };
        
        $scope.sendAdHocEmail = function ($event) {
            $event.preventDefault();

            var eventId = $scope.eventId;
            var emailSubject = $scope.emailSubject;
            var emailBody = angular.element('.summernote').eq(0).summernote("code");
            
            if (eventId && emailSubject && emailBody && $window.confirm("Are you sure to send an email to RSVPed students?")) {
                $http.post("/api/event/email", {
                    emails: $scope.event.students.map(function (x) { return x.student.email; }),
                    body: emailBody,
                    subject: emailSubject
                }).then(function () {
                    $window.alert("Successfully sent email!");
                });
            }
        };
        
        $scope.fetchInfo = function() {
          $http.get('/api/event/info/' + $scope.eventId).then(function (response) {
                var data = response.data;
                $scope.event = data.event;
                $scope.availableStudents = data.availableStudents;
            });
        };

        // Start the text editor
        angular.element('.summernote').summernote();

    }])
    .controller('userListCtrl', ['$scope', '$http', function ($scope, $http) {
        
    }])
    .controller('emailUtilityCtrl', ["$timeout", function ($timeout) {
        
        // Hide the .autoclose
        $timeout(function () {
            angular.element(".autoclose").fadeOut();
        }, 2000);  
        
        // Start the text editor
        angular.element('.summernote').summernote();
    }])
    .controller("emailCheckInCtrl", ['$scope', '$http', function ($scope, $http) {
        $scope.changeAttendance = function (type, id, value) {
            return $http.post("/utility/emailCheckInAction/" + type + "/" + id + "?present=" + value).then(function () { 
               console.log("Updated the attendance"); 
            });
        };
    }])
    .controller('hostEditCtrl', ['$scope', function ($scope) {

    }])
    .controller('hostRegistrationCtrl', ['$scope', function ($scope) {

    }])
    .controller('driverEditCtrl', ['$scope', function ($scope) {

    }])
    .controller('driverRegistrationCtrl', ['$scope', function ($scope) {

    }])
    .controller("studentRegistrationCtrl", ['$scope', function ($scope) {

    }])
    .controller('userRegistrationCtrl', ['$scope', function ($scope) {
        $scope.validateInvitationCode = function ($event) {
            if ($scope.invitation_code !== $scope.invitation_code_value) {
                $scope.error = true;
                $event.preventDefault();
            } else {
                $scope.error = false;
            }
        }
    }])
    .controller('studentListCtrl', ['$scope', '$http', function ($scope, $http) {
        
        $scope.showDetail = false;
        
        $scope.toggleShowDetail = function() { };
        
        $scope.getAllStudentsPDF = function () {
            $http.get("/api/student").then(function (response) {
                var students = response.data;

                var doc = new jsPDF({
                    orientation: "l",
                    lineHeight: 1.5
                });

                doc.setFont('courier');

                doc.setFontSize(10);

                var subsetAttr = function (attrList, obj) {
                    return attrList.reduce(function (o, k) {
                        o[k] = obj[k];
                        return o;
                    }, {});
                };

                var i, j, temparray, chunk = 25;
                for (i = 0, j = students.length; i < j; i += chunk) {
                    temparray = students.slice(i, i + chunk);

                    var str = stringTable.create(temparray.map(function (student) {
                        student.fullname = (student.fullname && student.fullname.substring(0, 30)) || '';
                        return subsetAttr(["fullname", "country", "university", "kosherFood", "needCarSeat", "isFamily", "isPresent"], student);
                    }));

                    // Needed
                    str = str.replace(/’/g, "'");

                    doc.text(20, 20, str);

                    if (i + chunk < j) {
                        doc.addPage();
                    }
                }

                doc.save("student-list.pdf");
            });
        };
    }])
    .controller("driverListCtrl", ['$scope', '$http', function ($scope, $http) {
        $scope.getAllDriversPDF = function () {
            $http.get("/api/driver").then(function (response) {
                var drivers = response.data;

                var doc = new jsPDF({
                    orientation: "l",
                    lineHeight: 1.5
                });

                doc.setFont('courier');

                doc.setFontSize(10);

                var subsetAttr = function (attrList, obj) {
                    return attrList.reduce(function (o, k) {
                        o[k] = obj[k];
                        return o;
                    }, {});
                };

                var i, j, temparray, chunk = 25;
                for (i = 0, j = drivers.length; i < j; i += chunk) {
                    temparray = drivers.slice(i, i + chunk);

                    var str = stringTable.create(temparray.map(function (driver) {
                        
                        // Set the navigator for the PDF
                        driver.navigator = (driver.navigator || driver.navigator === "null") ? 
                            (driver.navigator.length > 10 ? driver.navigator.substring(0, 10) + " ..." : driver.navigator) : "-";
                        
                        return subsetAttr(["displayId", "fullname", "capacity", "navigator", "role", "haveChildSeat"], driver);
                    }));
                    
                    // Needed
                    str = str.replace(/’/g, "'");

                    if (i === 0) {
                        str = "Driver List ( count of drivers: " + drivers.length + " )" + "\n\n" + str;
                    }

                    doc.text(20, 20, str);

                    if (i + chunk < j) {
                        doc.addPage();
                    }
                }

                doc.save("driver-list.pdf");
            });
        };
    }])
    .controller("hostListCtrl", ["$scope", "$http", function ($scope, $http) {
        $scope.getAllHostPDF = function() {
            $http.get("/api/host").then(function(response) {
                var hosts = response.data;

                var doc = new jsPDF({
                    orientation: "l",
                    lineHeight: 1.5
                });

                doc.setFont('courier');

                doc.setFontSize(10);

                var subsetAttr = function (attrList, obj) {
                    return attrList.reduce(function (o, k) {
                        o[k] = obj[k];
                        return o;
                    }, {});
                };

                var i, j, temparray, chunk = 25;
                for (i = 0, j = hosts.length; i < j; i += chunk) {
                    temparray = hosts.slice(i, i + chunk);

                    var str = stringTable.create(temparray.map(function (host) {
                        return subsetAttr(["fullname", "email", "phone", "address"], host);
                    }));

                    if (i === 0) {
                        str = "Host List ( count of hosts: " + hosts.length + " )" + "\n\n" + str;
                    }

                    doc.text(20, 20, str);

                    if (i + chunk < j) {
                        doc.addPage();
                    }
                }

                doc.save("host-list.pdf");
            });
        };
    }])
    .controller("studentDriverMappingCtrl", ["$scope", "$http", "$window", function ($scope, $http, $window) {

        $scope.showPresentOnly = false;

        $scope.togglePressentStudents = function(flag) {
            if (flag) {
                $scope.availableStudents = $scope.rawAvailableStudents.filter(function (student) {
                    return student.isPresent;
                });
            } else {
                $scope.availableStudents = $scope.rawAvailableStudents;
            }
        };
        
        $scope.getAllDriverMappingPDF = function() {
            $http.get("/api/studentDriverMapping/status").then(function(response) {
                var driverBucket = response.data.mappedDrivers;

                var doc = new jsPDF({
                    orientation: "l",
                    lineHeight: 1.5
                });

                doc.setFont('courier');

                doc.setFontSize(11);

                var subsetAttr = function(attrList, obj) {
                    return attrList.reduce(function(o, k) {
                        o[k] = obj[k];
                        return o;
                    }, {});
                };

                driverBucket.map(function(driver, index) {
                    var str = "";

                    if (driver) {
                        str += "Driver Name: " + driver.fullname + "\n";
                        str += "Driver Contact: " + driver.phone + "\n";
                        str += "Driver Capacity: " + driver.capacity + "\n";
                        str += "\n";
                    }

                    if (!driver.students) {
                        driver.students = [];
                    }
                    
                    str += stringTable.create(driver.students.map(function(driver) {
                        return subsetAttr(["fullname", "email", "phone", "country", "isPresent"], driver);
                    }));

                    doc.text(20, 20, str);

                    if (index + 1 < driverBucket.length) {
                        doc.addPage();
                    }
                });

                doc.save("student-driver-mapping.pdf");
            });
        };

        $scope.sendMailToDrivers = function($event) {
            if ($window.confirm("Are you sure to email mappings to drivers?")) {
                $http.post("/api/studentDriverMapping/EmailMappings").then(function (response) {
                    $window.alert("Successfully sent the mappings");
                });
            }
        };
        
        $scope.getStatus = function() {
            return $http.get("/api/studentDriverMapping/status").then(function(response) {
                var data = response.data;
                $scope.availableDrivers = data.availableDrivers;
                $scope.availableStudents = data.availableStudents;
                $scope.rawAvailableStudents = $scope.availableStudents;
                $scope.mappedDrivers = data.mappedDrivers;
                $scope.availableDriversBuckets = data.availableDrivers.reduce(function(rv, x) {
                    var key = ('host' in x && !!x['host']) ? x['host'].fullname : 'Unassigned';
                    (rv[key] = rv[key] || []).push(x);
                    return rv;
                }, {});
            });
        };

        $scope.map = function(studentId, driverId) {
            $scope.changeMap(studentId, driverId, "map");
        };

        $scope.unmap = function(studentId, driverId) {
            $scope.changeMap(studentId, driverId, "unmap");
        };

        $scope.changeMap = function(studentId, driverId, action) {
            if (driverId && studentId) {
                $http.post("/api/studentDriverMapping/" + action, {
                    "driverId": driverId,
                    "studentId": studentId
                }).then(function() {
                    $scope.getStatus();
                });
            }
        };

        $scope.init = function () {
            $scope.getStatus();
        };
        
        $scope.init();
    }])
    .controller("studentAttendanceCtrl", ["$scope", "$http", "$window", function ($scope, $http, $window) {
        $scope.countries = ["All Countries"];
        $scope.students = [];
        $scope.allStudents = [];
        
        $scope.country = "All Countries";
        $scope.attendanceFilter = "no";
        $scope.fullname = "";
        $scope.drivers = [];
        $scope.availableDriversBuckets = { };

        // Get All Drivers
        $scope.getAllDrivers = function () {
            $http.get("/api/driver/").then(function (response) {
                $scope.drivers = response.data.filter(function (value) { return value.role === 'Driver'; });
                $scope.availableDriversBuckets = $scope.drivers.reduce(function(rv, x) {
                    var key = ('host' in x && !!x['host']) ? x['host'].fullname : 'Unassigned';
                    (rv[key] = rv[key] || []).push(x);
                    return rv;
                }, {});
            });
        };
        
        $scope.addDriverMap = function(studentId, driverId) {
            if (driverId && studentId) {
                $http.post("/api/studentDriverMapping/map", {
                    "driverId": driverId,
                    "studentId": studentId
                }).then(function() {
                    $scope.getAllDrivers();
                    $scope.getAllStudents();
                });
            }
        };

        $scope.checkInViaEmail = function() {
            if ($window.confirm("Are you sure to send check-in via email to students?")) {
                return $http.post("/api/attendance/student/sendCheckIn").then(function () {
                    alert("Check-in via email is sent to students");
                });
            }
        };
        
        $scope.getAllStudents = function () {
            return $http.get("/api/student").then(function (response) {
                $scope.students = response.data;
                $scope.allStudents = response.data;

                $scope.students.forEach(function (student) {
                    if (!$scope.countries.includes(student.country)) {
                        $scope.countries.push(student.country);
                    }
                });

                // Filter
                var countries = $scope.countries.filter(function (value) { return value !== "All Countries" });
                
                // Sort
                countries.sort();
                
                $scope.countries = [ "All Countries" ].concat(countries);
                
                $scope.updateTable();
            });
        };

        $scope.changeAttendance = function (student) {
            return $http.post("/api/attendance/student/setAttendance", {
                id: student.id,
                attendance: student.isPresent
            }).then(function () {
                $scope.getAllStudents();
            });
        };

        $scope.updateTable = function () {
            var students = $scope.allStudents;
            
            var filteredStudents = {
                "country": [],
                "attendance": [],
                "fullname": []
            };
            
            if ($scope.country === "All Countries") {
                filteredStudents.country = students;
            } else {
                students.forEach(function (student) {
                    if (student.country === $scope.country) {
                        filteredStudents.country.push(student);
                    }
                });
            }
            
            students = filteredStudents.country;

            if ($scope.attendanceFilter === "all") {
                filteredStudents.attendance = students;
            } else {
                students.forEach(function (student) {
                    if ((student.isPresent && $scope.attendanceFilter === "yes") || (!student.isPresent && $scope.attendanceFilter === "no")) {
                        filteredStudents.attendance.push(student);
                    }
                });
            }
            
            students = filteredStudents.attendance;

            if (!$scope.fullname) {
                filteredStudents.fullname = students;
            } else {
                students.forEach(function (student) {
                    if (student.fullname.toLowerCase().indexOf($scope.fullname.toLowerCase()) > -1) {
                        filteredStudents.fullname.push(student);
                    }
                });
            }

            students = filteredStudents.fullname;
            
            $scope.students = students;
        };

        $scope.init = function () {
            $scope.getAllDrivers();
            $scope.getAllStudents();
        };

        $scope.init();
    }])
    .controller("driverHostMappingCtrl", ["$scope", "$http", "$window", function ($scope, $http, $window) {

        $scope.getHostInfo = function(host) {
            var hostCapacity = 0;
            var hostAssigned = 0;

            if (host.drivers) {
                hostCapacity = host.drivers.map(function (driver) { return driver.capacity })
                    .reduce(function (accumulator, currentValue) {  return accumulator + currentValue; });
                
                hostAssigned = host.drivers.map(function (driver) { return driver.students ? driver.students.length : 0})
                    .reduce(function (accumulator, currentValue) {  return accumulator + currentValue; });
            } 
            
            return {
                "hostCapacity": hostCapacity,
                "hostAssigned": hostAssigned
            };
        };
        
        $scope.getAllDriverMappingPDF = function() {
            $http.get("/api/driverHostMapping/status").then(function(response) {
                var hostBucket = response.data.mappedHosts;

                var doc = new jsPDF({
                    orientation: "l",
                    lineHeight: 1.5
                });

                doc.setFont('courier');

                doc.setFontSize(11);

                var subsetAttr = function(attrList, obj) {
                    return attrList.reduce(function(o, k) {
                        o[k] = obj[k];
                        return o;
                    }, {});
                };

                hostBucket.map(function(host, index) {
                    var str = "";

                    if (host) {
                        str += "Host Name: " + host.fullname + "\n";
                        str += "Host Address: " + host.address + "\n";
                        str += "Host Contact: " + host.phone + "\n";
                        str += "\n";
                    }

                    str += stringTable.create(host.drivers.map(function(driver) {
                        return subsetAttr(["fullname", "email", "phone", "capacity", "isPresent"], driver);
                    }));

                    doc.text(20, 20, str);

                    if (index + 1 < hostBucket.length) {
                        doc.addPage();
                    }
                });

                doc.save("driver-host-mapping.pdf");
            });
        };

        $scope.sendMailToHosts = function($event) {
            if ($window.confirm("Are you sure to email mappings to hosts?")) {
                $http.post("/api/driverHostMapping/EmailMappings").then(function (response) {
                    $window.alert("Successfully sent the mappings");
                });
            }
        };
        
        $scope.getStatus = function() {
            return $http.get("/api/driverHostMapping/status").then(function(response) {
                var data = response.data;
                $scope.availableDrivers = data.availableDrivers;
                $scope.availableHosts = data.availableHosts;
                $scope.mappedHosts = data.mappedHosts;
            });
        };

        $scope.map = function(driverId, hostId) {
            $scope.changeMap(driverId, hostId, "map");
        };

        $scope.unmap = function(driverId, hostId) {
            $scope.changeMap(driverId, hostId, "unmap");
        };

        $scope.changeMap = function(driverId, hostId, action) {
            if (driverId && hostId) {
                $http.post("/api/driverHostMapping/" + action, {
                    "driverId": driverId,
                    "hostId": hostId
                }).then(function() {
                    $scope.getStatus();
                });
            }
        };

        $scope.init = function () {
            $scope.getStatus();
        };

        $scope.init();
    }])
    .controller("driverAttendanceCtrl", ["$scope", "$http", "$window", function ($scope, $http, $window) {
        $scope.drivers = [];
        $scope.allDrivers = [];
        
        $scope.attendanceFilter = "all";
        $scope.fullname = "";

        $scope.checkInViaEmail = function() {
            if ($window.confirm("Are you sure to send check-in via email to drivers?")) {
                return $http.post("/api/attendance/driver/sendCheckIn").then(function () {
                    alert("Check-in via email is sent to drivers");
                });
            }
        };

        $scope.getAllDrivers = function () {
            return $http.get("/api/driver").then(function (response) {
                $scope.drivers = response.data.filter(function (value) { return value.role === 'Driver'; });
                $scope.allDrivers = response.data.filter(function (value) { return value.role === 'Driver'; });

                $scope.updateTable();
            });
        };

        $scope.changeAttendance = function (driver) {
            return $http.post("/api/attendance/driver/setAttendance", {
                id: driver.id,
                attendance: driver.isPresent
            }).then(function () {
                $scope.getAllStudents();
            });
        };

        $scope.updateTable = function () {
            var drivers = $scope.allDrivers;

            var filteredDrivers = {
                "attendance": [],
                "fullname": []
            };
            
            if ($scope.attendanceFilter === "all") {
                filteredDrivers.attendance = drivers;
            } else {
                drivers.forEach(function (driver) {
                    if ((driver.isPresent && $scope.attendanceFilter === "yes") || (!driver.isPresent && $scope.attendanceFilter === "no")) {
                        filteredDrivers.attendance.push(driver);
                    }
                });
            }

            drivers = filteredDrivers.attendance;

            if (!$scope.fullname) {
                filteredDrivers.fullname = drivers;
            } else {
                drivers.forEach(function (driver) {
                    if (driver.fullname.toLowerCase().indexOf($scope.fullname.toLowerCase()) > -1) {
                        filteredDrivers.fullname.push(driver);
                    }
                });
            }

            drivers = filteredDrivers.fullname;

            $scope.drivers = drivers;
        };

        $scope.init = function () {
            $scope.getAllDrivers();
        };

        $scope.init();
    }]);
    
