USE [master]
GO
/****** Object:  Database [Questionnaire]    Script Date: 2021/11/17 上午 08:58:37 ******/
CREATE DATABASE [Questionnaire]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Questionnaire', FILENAME = N'D:\課程練習用\MSSQL15.SQLEXPRESS\MSSQL\DATA\Questionnaire.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Questionnaire_log', FILENAME = N'D:\課程練習用\MSSQL15.SQLEXPRESS\MSSQL\DATA\Questionnaire_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [Questionnaire] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Questionnaire].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Questionnaire] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Questionnaire] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Questionnaire] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Questionnaire] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Questionnaire] SET ARITHABORT OFF 
GO
ALTER DATABASE [Questionnaire] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Questionnaire] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Questionnaire] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Questionnaire] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Questionnaire] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Questionnaire] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Questionnaire] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Questionnaire] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Questionnaire] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Questionnaire] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Questionnaire] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Questionnaire] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Questionnaire] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Questionnaire] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Questionnaire] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Questionnaire] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Questionnaire] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Questionnaire] SET RECOVERY FULL 
GO
ALTER DATABASE [Questionnaire] SET  MULTI_USER 
GO
ALTER DATABASE [Questionnaire] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Questionnaire] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Questionnaire] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Questionnaire] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [Questionnaire] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [Questionnaire] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'Questionnaire', N'ON'
GO
ALTER DATABASE [Questionnaire] SET QUERY_STORE = OFF
GO
USE [Questionnaire]
GO
/****** Object:  Table [dbo].[Information]    Script Date: 2021/11/17 上午 08:58:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Information](
	[UserID] [uniqueidentifier] NOT NULL,
	[Account] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Phone] [int] NOT NULL,
	[Email] [nvarchar](max) NOT NULL,
	[QID1] [int] NULL,
	[QID2] [int] NULL,
	[QID3] [int] NULL,
	[QID4] [int] NULL,
	[QID5] [int] NULL,
	[QID_All] [int] NOT NULL,
 CONSTRAINT [PK_Information] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Outline]    Script Date: 2021/11/17 上午 08:58:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Outline](
	[QuestionnaireID] [int] IDENTITY(1,1) NOT NULL,
	[QuestionnaireNum] [uniqueidentifier] NOT NULL,
	[Heading] [nvarchar](50) NOT NULL,
	[Vote] [nvarchar](10) NOT NULL,
	[StartTime] [datetime] NOT NULL,
	[EndTime] [datetime] NOT NULL,
	[Content] [nvarchar](max) NOT NULL,
	[Account] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Outline_1] PRIMARY KEY CLUSTERED 
(
	[QuestionnaireID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Question]    Script Date: 2021/11/17 上午 08:58:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Question](
	[QuestionnaireID] [int] NOT NULL,
	[TopicNum] [int] NOT NULL,
	[OptionsNum] [int] IDENTITY(1,1) NOT NULL,
	[answer1] [nvarchar](50) NULL,
	[answer2] [nvarchar](50) NULL,
	[answer3] [nvarchar](50) NULL,
	[answer4] [nvarchar](50) NULL,
	[answer5] [nvarchar](50) NULL,
	[answer6] [nvarchar](50) NULL,
	[answer7] [nvarchar](50) NULL,
	[answer8] [nvarchar](50) NULL,
	[answer9] [nvarchar](50) NULL,
	[answer10] [nvarchar](50) NULL,
	[OptionsAll] [int] NOT NULL,
 CONSTRAINT [PK_Question] PRIMARY KEY CLUSTERED 
(
	[OptionsNum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Questionnaires]    Script Date: 2021/11/17 上午 08:58:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Questionnaires](
	[QuestionnaireID] [int] NOT NULL,
	[TopicNum] [int] IDENTITY(1,1) NOT NULL,
	[TopicDescription] [nvarchar](50) NOT NULL,
	[TopicSummary] [nvarchar](150) NULL,
	[TopicType] [nvarchar](2) NOT NULL,
	[TopicMustKeyIn] [char](1) NOT NULL,
 CONSTRAINT [PK_Questionnaires_1] PRIMARY KEY CLUSTERED 
(
	[TopicNum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Record]    Script Date: 2021/11/17 上午 08:58:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Record](
	[RecordNum] [int] IDENTITY(1,1) NOT NULL,
	[QuestionnaireID] [int] NOT NULL,
	[AnswererID] [uniqueidentifier] NOT NULL,
	[AnsName] [nvarchar](50) NOT NULL,
	[AnsPhone] [int] NOT NULL,
	[AnsEmail] [nvarchar](50) NOT NULL,
	[AnsAge] [int] NOT NULL,
	[AnsTime] [datetime] NOT NULL,
 CONSTRAINT [PK_Record] PRIMARY KEY CLUSTERED 
(
	[RecordNum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Record Details]    Script Date: 2021/11/17 上午 08:58:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Record Details](
	[RecordNum] [int] NOT NULL,
	[QuestionnaireID] [int] NOT NULL,
	[TopicNum] [int] NOT NULL,
	[ReplicationNum] [int] IDENTITY(1,1) NOT NULL,
	[RDAns] [nvarchar](50) NULL,
 CONSTRAINT [PK_Record Details] PRIMARY KEY CLUSTERED 
(
	[ReplicationNum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[Information] ([UserID], [Account], [Password], [Name], [Phone], [Email], [QID1], [QID2], [QID3], [QID4], [QID5], [QID_All]) VALUES (N'8c228d0e-f435-452d-aa67-e37cb12f44e5', N'Clive123', N'123456789', N'Clive', 97463128, N'livelive@yahoo.com', 14, NULL, NULL, NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[Outline] ON 

INSERT [dbo].[Outline] ([QuestionnaireID], [QuestionnaireNum], [Heading], [Vote], [StartTime], [EndTime], [Content], [Account]) VALUES (14, N'a3e8aba8-2ce0-499b-a093-913cd9373fa6', N'學生焦慮行為分析', N'投票中', CAST(N'2021-10-01T00:00:00.000' AS DateTime), CAST(N'2021-12-15T00:00:00.000' AS DateTime), N'此問卷採取不記名方式，您所填寫的資料只作為學術性的分析，並不會將意見外流，請您安心填寫。', N'Clive123')
SET IDENTITY_INSERT [dbo].[Outline] OFF
GO
SET IDENTITY_INSERT [dbo].[Question] ON 

INSERT [dbo].[Question] ([QuestionnaireID], [TopicNum], [OptionsNum], [answer1], [answer2], [answer3], [answer4], [answer5], [answer6], [answer7], [answer8], [answer9], [answer10], [OptionsAll]) VALUES (14, 1, 1, N'顫抖', N'盜汗', N'易怒', N'失眠', N'無', NULL, NULL, NULL, NULL, NULL, 5)
INSERT [dbo].[Question] ([QuestionnaireID], [TopicNum], [OptionsNum], [answer1], [answer2], [answer3], [answer4], [answer5], [answer6], [answer7], [answer8], [answer9], [answer10], [OptionsAll]) VALUES (14, 2, 2, N'總是', N'經常', N'偶爾', N'幾乎沒有', NULL, NULL, NULL, NULL, NULL, NULL, 4)
SET IDENTITY_INSERT [dbo].[Question] OFF
GO
SET IDENTITY_INSERT [dbo].[Questionnaires] ON 

INSERT [dbo].[Questionnaires] ([QuestionnaireID], [TopicNum], [TopicDescription], [TopicSummary], [TopicType], [TopicMustKeyIn]) VALUES (14, 1, N'請問您有哪些焦慮行為呢?', NULL, N'CB', N'Y')
INSERT [dbo].[Questionnaires] ([QuestionnaireID], [TopicNum], [TopicDescription], [TopicSummary], [TopicType], [TopicMustKeyIn]) VALUES (14, 2, N'您多久感到焦慮呢?', NULL, N'RB', N'Y')
SET IDENTITY_INSERT [dbo].[Questionnaires] OFF
GO
SET IDENTITY_INSERT [dbo].[Record] ON 

INSERT [dbo].[Record] ([RecordNum], [QuestionnaireID], [AnswererID], [AnsName], [AnsPhone], [AnsEmail], [AnsAge], [AnsTime]) VALUES (1, 14, N'a2cc174a-419f-4f19-857c-6b539935cde8', N'Alexandra', 507525091, N'Alex@gmail.com', 26, CAST(N'2021-11-11T09:54:00.000' AS DateTime))
INSERT [dbo].[Record] ([RecordNum], [QuestionnaireID], [AnswererID], [AnsName], [AnsPhone], [AnsEmail], [AnsAge], [AnsTime]) VALUES (2, 14, N'2cc75fb2-8261-477a-8605-b1801c6d95bc', N'Tom', 400685221, N'Stavanger@gmail.com', 45, CAST(N'2021-11-11T09:58:00.000' AS DateTime))
INSERT [dbo].[Record] ([RecordNum], [QuestionnaireID], [AnswererID], [AnsName], [AnsPhone], [AnsEmail], [AnsAge], [AnsTime]) VALUES (3, 14, N'7d1bece6-e170-4820-9d05-520b308b1949', N'Amy', 123456789, N'hfksl@gmail.com
', 32, CAST(N'2021-10-25T00:00:00.000' AS DateTime))
SET IDENTITY_INSERT [dbo].[Record] OFF
GO
SET IDENTITY_INSERT [dbo].[Record Details] ON 

INSERT [dbo].[Record Details] ([RecordNum], [QuestionnaireID], [TopicNum], [ReplicationNum], [RDAns]) VALUES (1, 14, 1, 1, N'顫抖')
INSERT [dbo].[Record Details] ([RecordNum], [QuestionnaireID], [TopicNum], [ReplicationNum], [RDAns]) VALUES (2, 14, 1, 4, N'顫抖')
INSERT [dbo].[Record Details] ([RecordNum], [QuestionnaireID], [TopicNum], [ReplicationNum], [RDAns]) VALUES (3, 14, 1, 5, N'失眠')
INSERT [dbo].[Record Details] ([RecordNum], [QuestionnaireID], [TopicNum], [ReplicationNum], [RDAns]) VALUES (1, 14, 1, 6, N'易怒')
INSERT [dbo].[Record Details] ([RecordNum], [QuestionnaireID], [TopicNum], [ReplicationNum], [RDAns]) VALUES (2, 14, 1, 7, N'盜汗')
INSERT [dbo].[Record Details] ([RecordNum], [QuestionnaireID], [TopicNum], [ReplicationNum], [RDAns]) VALUES (3, 14, 1, 8, N'顫抖')
INSERT [dbo].[Record Details] ([RecordNum], [QuestionnaireID], [TopicNum], [ReplicationNum], [RDAns]) VALUES (1, 14, 2, 9, N'偶爾')
INSERT [dbo].[Record Details] ([RecordNum], [QuestionnaireID], [TopicNum], [ReplicationNum], [RDAns]) VALUES (2, 14, 2, 10, N'幾乎沒有')
INSERT [dbo].[Record Details] ([RecordNum], [QuestionnaireID], [TopicNum], [ReplicationNum], [RDAns]) VALUES (3, 14, 2, 11, N'幾乎沒有')
SET IDENTITY_INSERT [dbo].[Record Details] OFF
GO
ALTER TABLE [dbo].[Outline] ADD  CONSTRAINT [DF_Outline_StartTime]  DEFAULT (getdate()) FOR [StartTime]
GO
ALTER TABLE [dbo].[Outline] ADD  CONSTRAINT [DF_Outline_EndTime]  DEFAULT (getdate()) FOR [EndTime]
GO
USE [master]
GO
ALTER DATABASE [Questionnaire] SET  READ_WRITE 
GO
