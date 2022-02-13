const gulp = require('gulp');
const sass = require('gulp-sass')(require('sass'));
const autoprefixer = require('gulp-autoprefixer');

gulp.task('sass', () => {
    return gulp.src(['wwwroot/scss/site.scss'])
        .pipe(sass().on('error', sass.logError))
        .pipe(autoprefixer())
        .pipe(gulp.dest('wwwroot/css'));
});

// Default Task
gulp.task('default', gulp.series('sass'));