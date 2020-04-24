/// <binding Clean='clean' />
"use strict";

var gulp = require("gulp"),
    less = require("gulp-less"); // добавляем модуль less

var paths = {
    webroot: "./wwwroot/"
};
//  регистрируем задачу по преобразованию styles.less в файл css
gulp.task("less", function () {
    return gulp.src('less/styles.less')
        .pipe(less())
        .pipe(gulp.dest(paths.webroot + '/css'))
});