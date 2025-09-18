use std::fs::File;
use std::io;
use std::io::BufReader;
use std::io::prelude::*;

#[derive(Debug, Clone)]
struct Student {
    first_name: String,
    last_name: String,
    score: u32,
}

pub fn print_lines() -> io::Result<()> {
    let file = File::open("student_data.txt")?;
    let reader = BufReader::new(file);
    let mut students: Vec<Student> = Vec::new();

    for line in reader.lines() {
        let line = line?;
        let line_data: Vec<&str> = line.split_whitespace().collect();

        if line_data.len() == 3 {
            let student = create_student(line_data);
            students.push(student);
        }
    }

    let num_students = num_students(&students);
    if num_students > 0 {
        let avg_score = avg_score(&students);
        let (min_score, students_w_min) = min_score(&students);
        let (max_score, students_w_max) = max_score(&students);

        println!("number of students: {}", num_students);
        println!("average: {}", avg_score);
        println!("minimun score = {}", min_score);
        print_students(&students_w_min);
        println!("maximum score = {}", max_score);
        print_students(&students_w_max);
    } else {
        println!("No students found")
    }

    Ok(())
}

fn print_students(students: &Vec<Student>) {
    for student in students {
        println!("- {}, {}", student.last_name, student.first_name)
    }
}

fn create_student(student_data: Vec<&str>) -> Student {
    let first_name = student_data[0].to_string();
    let last_name = student_data[1].to_string();
    let score: u32 = student_data[2].parse().unwrap_or(0);
    Student {
        first_name,
        last_name,
        score,
    }
}

fn num_students(students: &Vec<Student>) -> u32 {
    students.len() as u32
}

fn avg_score(students: &Vec<Student>) -> f32 {
    let num_students: u32 = num_students(students);
    let mut sum_scores: u32 = 0;
    for student in students {
        sum_scores += student.score;
    }
    sum_scores as f32 / num_students as f32
}

fn min_score(students: &Vec<Student>) -> (u32, Vec<Student>) {
    let mut min_score: u32 = 100;
    let mut students_w_min: Vec<Student> = Vec::new();

    for student in students {
        match student.score {
            x if x < min_score => {
                min_score = x;
                students_w_min.clear();
                students_w_min.push(student.clone());
            }
            x if x == min_score => students_w_min.push(student.clone()),
            _ => {}
        }
    }
    (min_score, students_w_min)
}

fn max_score(students: &Vec<Student>) -> (u32, Vec<Student>) {
    let mut max_score: u32 = 0;
    let mut students_w_max: Vec<Student> = Vec::new();

    for student in students {
        match student.score {
            x if x > max_score => {
                max_score = x;
                students_w_max.clear();
                students_w_max.push(student.clone());
            }
            x if x == max_score => students_w_max.push(student.clone()),
            _ => {}
        }
    }
    (max_score, students_w_max)
}
