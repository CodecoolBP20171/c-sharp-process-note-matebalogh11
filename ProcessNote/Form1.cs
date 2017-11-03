﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ProcessNote
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        Process[] proc;
        Dictionary<string, string> comments = new Dictionary<string, string>();
        TextBox comment = new TextBox();
        string previousId;


        public void manageProcesses()
        {
            proc = Process.GetProcesses();
            foreach (Process process in proc)
            {
                dataGridView1.Rows.Add(process.ProcessName, process.Id);
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Hide();
            button1.Hide();
            textBox1.Hide();
            manageProcesses();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (previousId != null && (!comment.Text.Equals("You can add new comment here") && !comment.Text.Equals("Your comment was saved!")))
            {
                {
                    var confirmResult = MessageBox.Show("You have an unsaved comment. Press ok if you want to save it!",
                                                        "Confirm please",MessageBoxButtons.OKCancel);
                    if (confirmResult == DialogResult.OK)
                    {
                        comments.Add(previousId, comment.Text);
                    }
                }
            }
            var cell = dataGridView1.SelectedCells[0].Value.ToString();
            createComment();
            label1.Text = $"Name: {cell}";
            label1.Show();
            button1.Show();
            createProcAttr();
        }

        private void createComment()
        {
            /// build a textbox
            comment.Clear();
            comment.Multiline = true;
            comment.Top = 30;
            comment.Left = 280;
            comment.Width = 180;
            comment.Height = 120;
            string checkMemo = checkCommentValue();
            comment.Text = (checkMemo == null) ? "You can add new comment here" : checkMemo;
            this.Controls.Add(comment);
        }

        private void createProcAttr()
        {
            textBox1.Clear();
            textBox1.Show();
            int index = dataGridView1.SelectedCells[0].RowIndex;
            Process selectedProc = proc[index];
            string procTime;
            string memo;
            string vmemo;
            string pmemo;
            string priority;
            DateTime startTime;
            string startString;
            TimeSpan? runtime = null;

            try
            {
                procTime = selectedProc.UserProcessorTime.ToString();
                memo = selectedProc.PeakWorkingSet64.ToString();
                vmemo = selectedProc.PeakVirtualMemorySize64.ToString();
                pmemo = selectedProc.PeakPagedMemorySize64.ToString();
                priority = selectedProc.PriorityClass.ToString();
                startTime = selectedProc.StartTime;
                startString = startTime.ToString();
                runtime = DateTime.Now - startTime;
            } catch (System.ComponentModel.Win32Exception e)
            {
                procTime = memo = vmemo = pmemo = priority = startString = "unknown";
            }
            string run = (runtime != null) ? runtime.ToString() : "unkown";
            textBox1.Text = $"Processor time: {procTime}% {Environment.NewLine}Physical memory: {memo}B" +
                $"{Environment.NewLine}Virtual memory: {vmemo}B{Environment.NewLine}Paged memory: {pmemo}B" +
                $"{Environment.NewLine}Priority: {priority}{Environment.NewLine}Start time: {startString}" +
                $"{Environment.NewLine}Running time: {run}";

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comment.Text != null || !comment.Text.Equals("You can add new comment here"))
            {
                var cell = dataGridView1.SelectedCells[0];
                var row = dataGridView1.Rows[cell.RowIndex];
                var secondValue = row.Cells[1].Value;
                comments.Add(secondValue.ToString(), comment.Text);
                previousId = null;
                comment.Text = "Your comment was saved!";
            }
        }

        private string checkCommentValue()
        {
            var cell = dataGridView1.SelectedCells[0];
            var row = dataGridView1.Rows[cell.RowIndex];
            var secondValue = row.Cells[1].Value;
            previousId = secondValue.ToString();
            string currentComment;
            comments.TryGetValue(secondValue.ToString(), out currentComment);
            return currentComment;
        }
    }
}
